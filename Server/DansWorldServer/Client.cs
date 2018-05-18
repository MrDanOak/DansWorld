using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using DansWorld.Common.Net;
using DansWorld.Common.IO;
using DansWorld.Server.GameEntities;
using DansWorld.Common.Enums;
using DansWorld.Server.Data;
using System.Collections.Generic;
using DansWorld.Server.Utils;

namespace DansWorld.Server
{
    public class Client
    {
        #region public accessors
        /// <summary>
        /// Socket connection for the client
        /// </summary>
        public TcpClient Socket { get; private set; }
        /// <summary>
        /// Network stream of the client, used to read from and write to
        /// </summary>
        public NetworkStream Stream
        {
            get
            {
                try
                {
                    return Socket.GetStream();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
                return null;
            }
        }
        /// <summary>
        /// Listener thread for incoming data
        /// </summary>
        public Thread Thread { get; private set; }
        /// <summary>
        /// Bool to control whether the thread should stay alive
        /// </summary>
        public bool ShouldReceive { get; private set; }
        /// <summary>
        /// The account that this client is currently handling
        /// </summary>
        public Account Account;
        /// <summary>
        /// Pong flag. Used to detect whether a client has gone idle and is no longer responding to ping calls.
        /// </summary>
        public bool NeedPong = false;
        /// <summary>
        /// ID of the client assigned by the server
        /// </summary>
        public int ID;
        #endregion

        #region private accessors
        /// <summary>
        /// Flag to define whether packet data should be output to the console. Should NOT be used in production.
        /// </summary>
        private const bool PACKET_DEBUG = false;
        /// <summary>
        /// Locks the network stream so that only one packet may be sent at once ensuring that packets do not get scrambled.
        /// </summary>
        private static bool _isSending = false;
        /// <summary>
        /// Reference to the main server object so each client has awareness of other connected clients
        /// </summary>
        private Server _server;
        /// <summary>
        /// Database connection to allow saving of character and account states.
        /// </summary>
        private Database _database;
        /// <summary>
        /// The currently logged in player
        /// </summary>
        private PlayerCharacter _characterHandling;
        /// <summary>
        /// Used to prevent thread lock-ups.
        /// </summary>
        private int _timeout = 0;
        #endregion

        /// <summary>
        /// Constructor for the client object
        /// </summary>
        /// <param name="server">Server object</param>
        /// <param name="socket">TcpClient accepted by the listener</param>
        /// <param name="id">ID of the client assigned by the server</param>
        /// <param name="database">Database object</param>
        public Client(Server server, TcpClient socket, int id, Database database)
        {
            _server = server;
            Socket = socket;
            Thread = new Thread(new ThreadStart(Main));
            ShouldReceive = true;
            ID = id;
            _database = database;
        }

        /// <summary>
        /// Utility function used just to validate login data
        /// </summary>
        /// <param name="user">Username to check</param>
        /// <param name="pass">Password to check</param>
        private void CheckLogin(string user, string pass)
        {
            //return packet
            PacketBuilder pb = new PacketBuilder();
            Logger.Log(String.Format("Username: {0} Password: {1}", user, pass));
            //at the begining of our search we haven't found the account
            bool foundUser = false;
            foreach (Account account in _server.Accounts)
            {
                //when we have found the correct username
                if (account.Username == user)
                {
                    foundUser = true;
                    //and the correct password
                    if (account.Password == pass)
                    {
                        //we want to check if the account is already logged in
                        if (account.State == AccountState.LoggedOut)
                        {
                            //if it isn't already logged in then we want to grant access to the requesting client
                            Logger.Log("Login accepted from " + Socket.Client.RemoteEndPoint + " for account " + user);
                            pb = new PacketBuilder()
                                .AddByte((byte)PacketFamily.LOGIN)
                                .AddByte((byte)PacketAction.ACCEPT);
                            //here we're building up a catalogue of characters belonging to the requested account
                            foreach (PlayerCharacter character in account.Characters)
                            {
                                pb = pb.AddByte((byte)character.Name.Length)
                                    .AddString(character.Name)
                                    .AddByte((byte)character.Level)
                                    .AddByte((byte)character.Gender);
                            }
                            
                            account.State = AccountState.LoggedIn;
                            Account = account;
                        }
                        //if it is already logged in then we want to reject the login and let the user know that's the case
                        else
                        {
                            pb = new PacketBuilder()
                                .AddByte((byte)PacketFamily.LOGIN)
                                .AddByte((byte)PacketAction.REJECT)
                                .AddString("Account Logged in somewhere else");
                        }
                    }
                    //if the password is incorrect then we want to let the user know that's the case
                    else
                    {
                        pb = new PacketBuilder()
                            .AddByte((byte)PacketFamily.LOGIN)
                            .AddByte((byte)PacketAction.REJECT)
                            .AddString("Invalid Password");
                    }
                }
            }

            //if the username wasn't found in the known accounts then again we want to reject the login
            //along with the reason why
            if (!foundUser)
            {
                pb = new PacketBuilder()
                    .AddByte((byte)PacketFamily.LOGIN)
                    .AddByte((byte)PacketAction.REJECT)
                    .AddString("Invalid Username");
            }
            //sending the response back to the client whether it's an accepted login or a rejected login
            Send(pb.Build());
        }

        /// <summary>
        /// The purpose of this function is to provide one area where the packet data can be handled.
        /// I was looking to improve this approach by giving each packet its own class that handles 
        /// that type of packet. That would improve the clarity of this class.
        /// </summary>
        /// <param name="pkt">The packet to be handled</param>
        private void HandlePacket(Packet pkt)
        {
            try
            {
                //packet builder for the return packet
                PacketBuilder pb;
                if (pkt.Family == PacketFamily.LOGIN)
                {
                    if (pkt.Action == PacketAction.REQUEST)
                    {
                        //login request
                        string user = pkt.ReadString(pkt.ReadByte());
                        string pass = pkt.ReadString(pkt.ReadByte());
                        pass = Utils.Security.GetHashString(pass + "PROCO304" + user);
                        CheckLogin(user, pass);
                    }
                }
                else if (pkt.Family == PacketFamily.CHARACTER)
                {
                    if (pkt.Action == PacketAction.CREATE)
                    {
                        //for character creation, the first thing we must do is get the requested name
                        string characterName = pkt.ReadString(pkt.ReadByte());
                        //and then the requested gender
                        byte gender = pkt.ReadByte();
                        //and initialise whether the name is available or not
                        bool isAvailable = true;

                        foreach (Account account in _server.Accounts)
                        {
                            foreach (Character c in account.Characters)
                            {
                                //loop through every character known to the server
                                if (c.Name == characterName)
                                {
                                    //if the name already exists, then the requested name cannot be used
                                    isAvailable = false;
                                    break;
                                }
                            }
                        }

                        if (!isAvailable)
                        {
                            //relay back to the client to tell them that the name for the character already exists
                            pb = new PacketBuilder(PacketFamily.CHARACTER, PacketAction.ALREADY_EXISTS);
                            string errMsg = "Character already exists";
                            pb = pb.AddByte((byte)errMsg.Length).AddString(errMsg);
                            Send(pb.Build());
                        }
                        else
                        {
                            //if they can have the requested character name then we add it to the collection of characters
                            PlayerCharacter character = new PlayerCharacter()
                            {
                                Name = characterName,
                                Dexterity = 0,
                                Strength = 0,
                                Vitality = 0,
                                Intelligence = 0,
                                Gender = (Gender)gender,
                                Facing = 0, 
                                EXP = 0, 
                                Health = 50, 
                                Level = 0, 
                                X = 0, 
                                Y = 0
                            };
                            //and let the client know that they have been accepted
                            pb = new PacketBuilder(PacketFamily.CHARACTER, PacketAction.CREATED);
                            pb = pb.AddByte((byte)characterName.Length)
                                .AddString(characterName)
                                .AddByte((byte)character.Gender);
                            Send(pb.Build());
                            Account.Characters.Add(character);
                            //save the new character to the database also
                            character.CreateDatabaseEntry(_database, Account.Username);
                            //and then put it out to the server console
                            Logger.Log("Character Created. Name: " + character.Name);
                        }
                    }
                }
                else if (pkt.Family == PacketFamily.REGISTER)
                {
                    if (pkt.Action == PacketAction.REQUEST)
                    {
                        //registering to the game
                        //getting the username
                        string username = pkt.ReadString(pkt.ReadByte());
                        //getting the password
                        string password = pkt.ReadString(pkt.ReadByte());
                        //hashing the given password using the username and "PROCO304" as the salt
                        password = Utils.Security.GetHashString(password + "PROCO304" + username);
                        //read the given email
                        string email = pkt.ReadString(pkt.ReadByte());
                        //and the given name
                        string fullname = pkt.ReadString(pkt.ReadByte());
                        //output the requested account to the server console
                        Logger.Log(String.Format("Requested username {0} password {1} email {2}", username, password, email));
                        //initiate the test to see if the account username already exists
                        bool userExists = false;
                        foreach (Account account in _server.Accounts)
                        {
                            if (account.Username == username)
                            {
                                //if the requested account name already exists in the known acccounts list then we need to reject the request
                                //and post back to the client to tell them that the username is already taken.
                                pb = new PacketBuilder(PacketFamily.REGISTER, PacketAction.REJECT);
                                string errMsg = "User already exists";
                                pb = pb.AddByte((byte)errMsg.Length).AddString(errMsg);
                                Send(pb.Build());
                                userExists = true;
                                break;
                            }
                        }

                        if (!userExists)
                        {
                            //if the account isn't already taken then we can accept the registration request, 
                            //create the new entry in the database and report back to the client to tell them that 
                            //their account is ready to go
                            pb = new PacketBuilder(PacketFamily.REGISTER, PacketAction.ACCEPT);
                            pb = pb.AddByte((byte)username.Length).AddString(username);
                            Send(pb.Build());
                            Account account = new Account(username, password, email, fullname);
                            _server.Accounts.Add(account);
                            account.CreateDatabaseEntry(_database, Socket.Client.RemoteEndPoint.ToString().Split(":")[0]);
                            Logger.Log("Account Created. Username: " + username);
                        }
                    }
                }
                else if (pkt.Family == PacketFamily.PLAY)
                {
                    if (pkt.Action == PacketAction.REQUEST)
                    {
                        //with this section of code we are getting a request to 
                        //play a selected character within an account
                        //userid is the id of the character they wish to play
                        int userid = pkt.ReadByte();
                        if (Account.Characters.Count >= userid)
                        {
                            pb = new PacketBuilder(PacketFamily.PLAY, PacketAction.ACCEPT);
                            //retrieve the selected character
                            PlayerCharacter c = Account.GetCharacter(userid);
                            _characterHandling = c;
                            c.ServerID = ID;
                            //give back the character to the game client so it can 
                            //be the first character in its known characters array
                            pb = pb.AddByte((byte)c.Name.Length).AddString(c.Name)
                                   .AddByte((byte)c.Level)
                                   .AddByte((byte)c.Gender)
                                   .AddInt(c.X)
                                   .AddInt(c.Y)
                                   .AddByte((byte)c.Facing)
                                   .AddInt(c.ServerID).AddInt(_server.LoggedInPlayers.Count);

                            //giving the client every character it has knowledge of
                            foreach (PlayerCharacter character in _server.LoggedInPlayers)
                            {
                                pb = pb.AddByte((byte)character.Name.Length).AddString(character.Name)
                                   .AddByte((byte)character.Level)
                                   .AddByte((byte)character.Gender)
                                   .AddInt(character.X)
                                   .AddInt(character.Y)
                                   .AddByte((byte)character.Facing)
                                   .AddInt(character.ServerID);
                            }
                            Send(pb.Build());

                            //adds the character to the logged in players
                            _server.LoggedInPlayers.Add(c);

                            foreach (Client client in _server.Clients)
                            {
                                if (client != this)
                                {
                                    //giving every other client knowledge of the newly logged in character
                                    pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.WELCOME);
                                    pb = pb.AddByte((byte)c.Name.Length).AddString(c.Name)
                                           .AddByte((byte)c.Level)
                                           .AddByte((byte)c.Gender)
                                           .AddInt(c.X)
                                           .AddInt(c.Y)
                                           .AddByte((byte)c.Facing)
                                           .AddInt(c.ServerID);
                                    client.Send(pb.Build());
                                }
                            }
                            int servID = 0;
                            foreach (Enemy enemy in _server.Enemies)
                            {
                                //giving the client knowledge of all enemies
                                //this should be changed to only give knowlegde of enemies within seeing distance of the client
                                pb = new PacketBuilder(PacketFamily.ENEMY, PacketAction.WELCOME);
                                pb = pb.AddInt(_server.Enemies.IndexOf(enemy))
                                    .AddByte((byte)enemy.Name.Length)
                                    .AddString(enemy.Name)
                                    .AddByte((byte)enemy.Facing)
                                    .AddInt(enemy.X)
                                    .AddInt(enemy.Y)
                                    .AddInt(enemy.Vitality)
                                    .AddByte((byte)enemy.Level)
                                    .AddInt(enemy.MaxHealth)
                                    .AddInt(enemy.Health)
                                    .AddInt(enemy.SpriteID)
                                    .AddByte((byte)servID++);
                                Send(pb.Build());
                            }
                            //updating the account state to playing
                            Account.State = AccountState.Playing;
                        }
                    }
                }
                else if (pkt.Family == PacketFamily.PLAYER)
                {
                    if (pkt.Action == PacketAction.MOVE || pkt.Action == PacketAction.STOP)
                    {
                        //used as a relay for all clients in the game.
                        //gives every client the same packet that it received from the source client
                        int x = pkt.ReadInt();
                        int y = pkt.ReadInt();
                        Direction facing = (Direction)pkt.ReadByte();
                        int id = pkt.ReadInt();
                        if (_characterHandling.ServerID == id && (_characterHandling.X != x || _characterHandling.Y != y))
                        {
                            _characterHandling.X = x;
                            _characterHandling.Y = y;
                            _characterHandling.Facing = facing;
                        }

                        foreach (Client client in _server.Clients)
                        {
                            client.Send(pkt);
                        }
                    }
                    //only peeking the int here because we do not want to move the read pointer
                    else if (pkt.Action == PacketAction.LOGOUT && pkt.PeekInt() == _characterHandling.ServerID)
                    {
                        //handles character logging out
                        LogOut(_characterHandling);
                    }
                    else if (pkt.Action == PacketAction.TALK)
                    {
                        foreach (Client client in _server.Clients)
                        {
                            //chat client relay
                            //sends every client the same packet that it received
                            client.Send(pkt);
                        }
                    }
                    else if (pkt.Action == PacketAction.ATTACK)
                    {
                        foreach (Enemy enemy in _server.Enemies)
                        {
                            bool enemyhit = false;
                            switch (_characterHandling.Facing)
                            {
                                case Direction.LEFT:
                                    if (enemy.X > _characterHandling.X - (48 * 2) && 
                                        enemy.X < _characterHandling.X + 24 && 
                                        enemy.Y > _characterHandling.Y - 48 && 
                                        enemy.Y < _characterHandling.Y + (48 * 2))
                                    {
                                        enemyhit = true;
                                        enemy.Health -= 5;
                                    }
                                    break;
                                case Direction.DOWN:
                                    if (enemy.X < _characterHandling.X + (48 * 2) &&
                                        enemy.X > _characterHandling.X - 48 &&
                                        enemy.Y > _characterHandling.Y + 48 &&
                                        enemy.Y < _characterHandling.Y + (48 * 2))
                                    {
                                        enemyhit = true;
                                        enemy.Health -= 5;
                                    }
                                    break;
                                case Direction.RIGHT:
                                    if (enemy.X < _characterHandling.X + (48 * 2) &&
                                        enemy.X > _characterHandling.X + 24 &&
                                        enemy.Y > _characterHandling.Y - 48 &&
                                        enemy.Y < _characterHandling.Y + (48 * 2))
                                    {
                                        enemyhit = true;
                                        enemy.Health -= 5;
                                    }
                                    break;
                                case Direction.UP:
                                    if (enemy.X < _characterHandling.X + (48 * 2) &&
                                       enemy.X > _characterHandling.X - 48 &&
                                       enemy.Y > _characterHandling.Y - (48 * 2) &&
                                       enemy.Y < _characterHandling.Y)
                                    {
                                        enemyhit = true;
                                        enemy.Health -= 5;
                                    }
                                    break;
                            }
                            if (enemyhit)
                            {
                                //ensuring the player gets exp credit for the kill
                                if (!enemy.Contributors.Contains(_characterHandling))
                                {
                                    enemy.Contributors.Add(_characterHandling);
                                }

                                //enemy died
                                if (enemy.Health <= 0)
                                {
                                    //reward exp to all contributors
                                    foreach (PlayerCharacter player in enemy.Contributors)
                                    {
                                        player.EXP += enemy.EXP;
                                        _server.Announce(player.Name + " has gained " + enemy.EXP + " EXP");
                                    }

                                    enemy.Contributors = new List<PlayerCharacter>();

                                    //reset position
                                    enemy.X = enemy.SpawnX + RNG.Next(-300, 300);
                                    enemy.Y = enemy.SpawnY + RNG.Next(-300, 300);
                                    enemy.Health = enemy.MaxHealth;
                                    //let all clients know of new poisition
                                    pb = new PacketBuilder(PacketFamily.ENEMY, PacketAction.MOVE);
                                    pb = pb.AddInt(_server.Enemies.IndexOf(enemy))
                                            .AddInt(enemy.X)
                                            .AddInt(enemy.Y)
                                            .AddByte((byte)enemy.Facing);
                                    foreach (Client client in _server.Clients)
                                    {
                                        client.Send(pb.Build());
                                    }
                                }

                                //notify all clients of new enemy hp
                                pb = new PacketBuilder(PacketFamily.ENEMY, PacketAction.TAKE_DAMAGE)
                                       .AddInt(_server.Enemies.IndexOf(enemy))
                                       .AddInt(enemy.Health);
                                foreach (Client client in _server.Clients)
                                {
                                    client.Send(pb.Build());
                                }
                            }
                        }
                    }
                }
                else if (pkt.Family == PacketFamily.CONNECTION)
                {
                    if (pkt.Action == PacketAction.PONG)
                    {
                        //Good client, responded to the PING request. We no longer need the pong.
                        NeedPong = false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Packet handling error. Remote endpoint: "
                    + Socket.Client.RemoteEndPoint + "\n\r StackTrace:" + e.StackTrace + "\n\r Message:" + e.Message);
            }
        }

        public void Main() 
        {
            Logger.Log(String.Format("Client handler thread spawned id:{0}", Thread.CurrentThread.ManagedThreadId));
            while (ShouldReceive) 
            {
                try
                {
                    //the main function that receives packet data
                    byte[] buffer = new byte[2];
                    int readTotal = 0;
                    //reading the length of the packet
                    Stream.Read(buffer, 0, 2);
                    if (buffer[0] == 0)
                        return;

                    byte length = buffer[0];
                    if (length < 2)
                    {
                        //we're not interested in any packets less than 2. 
                        //Could happen if a client other than a DansWorld client tries to connect.
                        Logger.Error("Packet of length less than 2 received");
                        return;
                    }

                    //initiating a new byte array of the packet length
                    byte[] data = new byte[length];

                    //while there's still data to read
                    while (readTotal != length)
                    {
                        //filling the byte array with the data read from the stream
                        int read = Stream.Read(data, 0, length);
                        if (read == 0)
                        {
                            Logger.Warn("Packet seems empty..");
                            return;
                        }
                        readTotal += read;
                    }

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < data.Length; i++) { sb.Append("{" + data[i] + "} "); }
                    if (PACKET_DEBUG)
                        Logger.Log("Packet length: " + length + " Packet data: " + sb.ToString());

                    //building up the received packet data using the received byte array
                    PacketBuilder pb = new PacketBuilder().AddBytes(data);
                    Packet pkt = pb.Build();
                    
                    HandlePacket(pkt);
                }
                catch (Exception e)
                {
                    Logger.Error("Fatal Exception: " + e.Message);
                    Stop();
                    break;
                }
            }
        }

        /// <summary>
        /// Start the listening thread for incoming data
        /// </summary>
        public void Start() 
        {
            if (!Thread.IsAlive)
                Thread.Start();
        }

        /// <summary>
        /// Send data to the connected client
        /// </summary>
        /// <param name="data">the byte array to be sent</param>
        public void Send(byte[] data)
        {
            try
            {
                //locking this read while the client is already sending something else.
                while (_isSending && _timeout < 1000) { _timeout += 1; Thread.Sleep(1); }

                _timeout = 0;
                _isSending = true;
                byte length = (byte)data.Length;
                Stream.Write(new byte[] { length, 0 }, 0, 2);
                Stream.Write(data, 0, length);
                _isSending = false;

                if (PACKET_DEBUG)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in data)
                    {
                        sb.Append($"[{b}] ");
                    }
                    Logger.Log($"Length: {length} Data: {sb.ToString()}");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Fatal Exception: " + e.Message);
                Stop();
            }
        }

        /// <summary>
        /// Overloaded method to send a string on its own
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }

        /// <summary>
        /// Overloaded method to send a constructed packet
        /// </summary>
        /// <param name="packet"></param>
        public void Send(Packet packet)
        {
            Send(packet.RawData);
        }
        
        /// <summary>
        /// Public method to join any outside threads into this current thread execution
        /// </summary>
        public void Join()
        {
            if (Thread.IsAlive)
                Thread.Join();
        }

        /// <summary>
        /// Method to handle logging out of the character
        /// </summary>
        /// <param name="character">Character to be logged out</param>
        public void LogOut(PlayerCharacter character)
        {
            //saves the character state
            character.Save(_database);
            character.ServerID = 0;
            PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.LOGOUT);
            pb = pb.AddInt(character.ServerID);
            foreach (Client client in _server.Clients)
            {
                if (client.Socket.Connected)
                {
                    //lets all the connected clients know that this character has logged out
                    if (client != this)
                        client.Send(pb.Build());
                }
            }
        }

        /// <summary>
        /// shuts the client down
        /// </summary>
        public void Stop()
        {
            //making sure that the account is set to logged out in case the user wishes to log in again at a later date
            if (Account != null) Account.State = AccountState.LoggedOut;

            //removing the character from the logged in players
            if (_server.LoggedInPlayers.Contains(_characterHandling)) _server.LoggedInPlayers.Remove(_characterHandling);

            //notifying every connected client that the character has disconnected
            //This should de-render the character if it is shown to any client
            if (_characterHandling != null)
            {
                foreach (Client client in _server.Clients)
                {
                    if (client == this) continue;
                    PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.LOGOUT)
                        .AddInt(_characterHandling.ServerID);

                    client.Send(pb.Build());
                }
                //ensuring the character is saved upon client shutdown
                LogOut(_characterHandling);
            }

            Logger.Log("Client iniating shutdown");
            if (_server.Clients.Contains(this))
            {
                //ensuring that this client won't attempt to send any more data to a dead endpoint
                _server.Remove(this);
            }

            try
            {
                //cleanup of class resources
                if (Socket != null && Socket.Connected)
                {
                    Socket.GetStream().Dispose();
                    Socket.Client.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }

                if (Thread != null && Thread.IsAlive)
                {
                    ShouldReceive = false;
                    Thread.Interrupt();
                    Thread.Join();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message.ToString());
            }
        }
    }
}
