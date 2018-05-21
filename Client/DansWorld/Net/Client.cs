using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using DansWorld.Common.IO;
using DansWorld.Common.Net;
using DansWorld.Common.GameEntities;
using DansWorld.Common.Enums;
using System.Globalization;

namespace DansWorld.GameClient.Net
{
    public class Client
    {
        /// <summary>
        /// Display packet data out to the console?
        /// </summary>
        private const bool PACKET_DEBUG = false;
        /// <summary>
        /// server address
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// server port
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// TcpClient provides us with the network stream to write to
        /// </summary>
        public TcpClient Socket { get; private set; }
        /// <summary>
        /// Getting the network strteam from the socket
        /// </summary>
        public NetworkStream Stream { get { if (Socket.Connected) return Socket.GetStream(); else return null; } }
        /// <summary>
        /// Are we connected?
        /// </summary>
        public bool Connected { get { return Socket.Connected; } }
        /// <summary>
        /// Should the worker thread stay alive?
        /// </summary>
        public bool ShouldReceive { get; private set; }
        /// <summary>
        /// Thread for the worker thread to exist in for the blocking functions
        /// </summary>
        public Thread Thread { get; private set; }
        /// <summary>
        /// Reference to the game client so that the game scene may be switched depending on incoming packet data
        /// </summary>
        private GameClient _gameClient;

        public Client(string host, int port, GameClient gameClient)
        {
            Host = host;
            Port = port;
            ShouldReceive = true;
            Socket = new TcpClient();
            Thread = new Thread(new ThreadStart(Main));
            _gameClient = gameClient;
        }

        public void Connect()
        {
            try
            {
                Socket = new TcpClient();
                Socket.Connect(Dns.GetHostAddresses(Host)[0], Port);
                Thread.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " Stack: " + e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }

        public void Main()
        {
            while (ShouldReceive)
            {
                try
                {
                    if (Stream == null)
                        return;

                    //the main function that receives packet data
                    byte[] buffer = new byte[2];
                    int readTotal = 0;
                    //reading the length of the packet
                    Stream.Read(buffer, 0, 2);
                    if (buffer[0] == 0)
                        continue;

                    byte length = buffer[0];
                    if (length < 2 && PACKET_DEBUG)
                    {
                        //we're not interested in any packets less than 2. 
                        //Could happen if a client other than a DansWorld client tries to connect.
                        Logger.Error("Packet of length less than 2 received");
                        continue;
                    }

                    //initiating a new byte array of the packet length
                    byte[] data = new byte[length];

                    //while there's still data to read
                    while (readTotal < length)
                    {
                        //filling the byte array with the data read from the stream
                        int read = Stream.Read(data, 0, length);
                        if (read == 0 && PACKET_DEBUG)
                        {
                            Logger.Warn("Packet seems empty..");
                            return;
                        }
                        readTotal += read;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Packet Data: ");
                    for (int i = 0; i < data.Length; i++) { sb.Append("{" + data[i] + "} "); }

                    if (PACKET_DEBUG)
                        Logger.Log(sb.ToString());

                    //building up the received packet data using the received byte array
                    PacketBuilder pb = new PacketBuilder().AddBytes(data);
                    Packet pkt = pb.Build();

                    //if we have an accepted login packet
                    if (pkt.Family == PacketFamily.LOGIN)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            //debug output
                            _gameClient.DisplayMessage("Login accepted");
                            //we're going to set the state of the client to logged in
                            _gameClient.SetState(GameExecution.GameState.LoggedIn);
                            //while we still have players to read the data of
                            while (pkt.ReadPosition < pkt.RawData.Length)
                            {
                                int nameLength = pkt.ReadByte();
                                if (nameLength > 0)
                                {
                                    PlayerCharacter c = new PlayerCharacter()
                                    {
                                        Name = pkt.ReadString(nameLength),
                                        Level = pkt.ReadByte(),
                                        Gender = (Gender)pkt.ReadByte()
                                    };
                                    _gameClient.AddPlayerCharacter(c);
                                }
                            }
                        }
                        else
                        {
                            //if the login wasn't accepted, the person should get the reason as to why
                            _gameClient.DisplayMessage(pkt.ReadString(pkt.Length - 2));
                        }
                    }
                    else if (pkt.Family == PacketFamily.REGISTER)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            //registration was complete, we can now go back to the main menu to log in to the created account
                            _gameClient.SetState(GameExecution.GameState.MainMenu);
                            _gameClient.DisplayMessage("Account created! Username: " + pkt.ReadString(pkt.ReadByte()));
                        }
                        else
                        {
                            //if registration was not completed, the client needs to know why (username in use)
                            _gameClient.DisplayMessage(pkt.ReadString(pkt.ReadByte()));
                        }
                    }
                    else if (pkt.Family == PacketFamily.CHARACTER)
                    {
                        if (pkt.Action == PacketAction.CREATED)
                        {
                            PlayerCharacter player = new PlayerCharacter()
                            {
                                Name = pkt.ReadString(pkt.ReadByte()),
                                Gender = (Gender)pkt.ReadByte()
                            };
                            //if the character has been created we want to update the character select screen to include the new character
                            _gameClient.CharacterSelect.AddCharacter(player);
                            _gameClient.SetState(GameExecution.GameState.LoggedIn);
                        }
                        else if (pkt.Action == PacketAction.ALREADY_EXISTS)
                        {
                            //if the character already exists then we need to inform the client that it can't use the name
                            string message = pkt.ReadString(pkt.ReadByte());
                            _gameClient.DisplayMessage(message);
                        }
                        else if (pkt.Action == PacketAction.DELETE)
                        {
                            //if the character is to be deleted, then it needs to be removed from the client display
                            _gameClient.SetState(GameExecution.GameState.LoggedIn);
                            _gameClient.CharacterSelect.RemoveCharacter(pkt.ReadByte());
                        }
                    }
                    else if (pkt.Family == PacketFamily.PLAY)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            //loading into the game 
                            PlayerCharacter player = new PlayerCharacter()
                            {
                                Name = pkt.ReadString(pkt.ReadByte()),
                                Level = pkt.ReadByte(),
                                Gender = (Gender)pkt.ReadByte(),
                                X = pkt.ReadInt(), 
                                Y = pkt.ReadInt(), 
                                Facing = (Direction)pkt.ReadByte(), 
                                ID = pkt.ReadInt()
                            };
                            _gameClient.ClearCharacters();
                            _gameClient.SetState(GameExecution.GameState.Playing);
                            _gameClient.CharacterID = player.ID;
                            _gameClient.AddPlayerCharacter(player);
                            int loggedInCharacters = pkt.ReadInt();
                            for (int i = 0; i < loggedInCharacters; i++)
                            {
                                //adding all currently logged in players to the game evironment
                                PlayerCharacter p = new PlayerCharacter()
                                {
                                    Name = pkt.ReadString(pkt.ReadByte()),
                                    Level = pkt.ReadByte(),
                                    Gender = (Gender)pkt.ReadByte(),
                                    X = pkt.ReadInt(),
                                    Y = pkt.ReadInt(),
                                    Facing = (Direction)pkt.ReadByte(),
                                    ID = pkt.ReadInt()
                                };
                                _gameClient.AddPlayerCharacter(p);
                            }
                        }
                    }
                    else if (pkt.Family == PacketFamily.PLAYER)
                    {
                        // a new player has logged in
                        if (pkt.Action == PacketAction.WELCOME)
                        {
                            PlayerCharacter player = new PlayerCharacter()
                            {
                                Name = pkt.ReadString(pkt.ReadByte()),
                                Level = pkt.ReadByte(),
                                Gender = (Gender)pkt.ReadByte(),
                                X = pkt.ReadInt(), 
                                Y = pkt.ReadInt(),
                                Facing = (Direction)pkt.ReadByte(),
                                ID = pkt.ReadInt()
                            };
                            _gameClient.AddPlayerCharacter(player);
                        }
                        //a player has moved
                        else if (pkt.Action == PacketAction.MOVE)
                        {
                            int x = pkt.ReadInt();
                            int y = pkt.ReadInt();
                            Direction d = (Direction)pkt.ReadByte();
                            int id = pkt.ReadInt();
                            foreach (PlayerCharacter player in _gameClient.GetPlayers())
                            {
                                if (player.ID == id && _gameClient.CharacterID != id)
                                {
                                    player.X = x;
                                    player.Y = y;
                                    player.IsWalking = true;
                                    player.IsIdle = false;
                                    player.Facing = d;
                                    break;
                                }
                            }
                        }
                        //a player has stopped moving
                        else if (pkt.Action == PacketAction.STOP)
                        {
                            int x = pkt.ReadInt();
                            int y = pkt.ReadInt();
                            Direction d = (Direction)pkt.ReadByte();
                            int id = pkt.ReadInt();
                            foreach (PlayerCharacter player in _gameClient.GetPlayers())
                            {
                                if (player.ID == id && _gameClient.CharacterID != id)
                                {
                                    player.X = x;
                                    player.Y = y;
                                    player.IsWalking = false;
                                    player.IsIdle = true;
                                    player.Facing = d;
                                    break;
                                }
                            }
                        }
                        //a player has logged out
                        else if (pkt.Action == PacketAction.LOGOUT)
                        {
                            PlayerCharacter toRemove = null;
                            foreach (PlayerCharacter player in _gameClient.GetPlayers())
                            {
                                if (player.ID == pkt.PeekInt())
                                    toRemove = player;
                            }
                            if (toRemove != null)
                                _gameClient.RemoveCharacter(toRemove);
                        }
                        //a player has sent a message
                        else if (pkt.Action == PacketAction.TALK)
                        {
                            PlayerCharacter p = null;
                            string message = pkt.ReadString(pkt.ReadInt());
                            int id = pkt.ReadInt();
                            foreach (PlayerCharacter player in _gameClient.GetPlayers())
                            {
                                if (player.ID == id)
                                {
                                    p = player;
                                }
                            }
                            _gameClient.DisplayMessage(message, (p == null ? "" : p.Name));
                        }
                    }
                    //server sent an announcement
                    else if (pkt.Family == PacketFamily.SERVER)
                    {
                        if (pkt.Action == PacketAction.TALK)
                        {
                            string message = pkt.ReadString(pkt.ReadInt());
                            _gameClient.DisplayMessage(message, "SERVER");
                        }
                    }
                    //if we got a ping request, we'll send back a pong request to satisfy the server
                    else if (pkt.Family == PacketFamily.CONNECTION)
                    {
                        if (pkt.Action == PacketAction.PING)
                        {
                            PacketBuilder pBuilder = new PacketBuilder(PacketFamily.CONNECTION, PacketAction.PONG);
                            DateTime now = DateTime.Now.ToUniversalTime();
                            DateTime fromServer = DateTime.ParseExact(pkt.ReadString(pkt.ReadInt()), "hh.mm.ss.ffffff", CultureInfo.InvariantCulture);
                            TimeSpan t = now.Subtract(fromServer);
                            string nowString = now.ToShortTimeString();
                            pBuilder = pBuilder.AddInt(nowString.Length).AddString(nowString);
                            Send(pBuilder.Build());
                            _gameClient.ShowPing(t.Milliseconds);
                        }
                    }
                    else if (pkt.Family == PacketFamily.ENEMY)
                    {
                        //an enemy has been added to the game
                        if (pkt.Action == PacketAction.WELCOME)
                        {
                            Enemy enemy = new Enemy();
                            enemy.ID = pkt.ReadInt();
                            enemy.Name = pkt.ReadString(pkt.ReadByte());
                            enemy.Facing = (Direction)pkt.ReadByte();
                            enemy.X = pkt.ReadInt();
                            enemy.Y = pkt.ReadInt();
                            enemy.Vitality = pkt.ReadInt();
                            enemy.Level = pkt.ReadByte();
                            enemy.MaxHealth = pkt.ReadInt();
                            enemy.Health = pkt.ReadInt();
                            enemy.SpriteID = pkt.ReadInt();
                            enemy.ServerID = pkt.ReadByte();
                            _gameClient.AddEnemy(enemy);
                        }
                        //an enemy moved
                        else if (pkt.Action == PacketAction.MOVE)
                        {
                            int id = pkt.ReadInt();
                            int x = pkt.ReadInt();
                            int y = pkt.ReadInt();
                            byte facing = pkt.ReadByte();
                            foreach (Enemy enemy in _gameClient.GetEnemies())
                            {
                                if (enemy.ServerID == id)
                                {
                                    enemy.X = x;
                                    enemy.Y = y;
                                    enemy.Facing = (Direction)facing;
                                    break;
                                }
                            }
                        }
                        //an enemy took damage
                        else if (pkt.Action == PacketAction.TAKE_DAMAGE)
                        {
                            int id = pkt.ReadInt();
                            int hp = pkt.ReadInt();

                            foreach (Enemy enemy in _gameClient.GetEnemies())
                            {
                                if (enemy.ID == id)
                                {
                                    enemy.Health = hp;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message + " Stack " + e.StackTrace);
                    continue;
                }
            }
        }

        public void Send(byte[] data)
        {
            int tries = 0;
            bool success = false;

            //attempting to send 10 times before it gives up
            while (!success && tries < 10)
            {
                try
                {
                    byte length = (byte)data.Length;
                    Stream.Write(new byte[] { length, 0 }, 0, 2);
                    Stream.Write(data, 0, length);
                    success = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message + " Stack " + e.StackTrace);
                    tries++;
                    Connect();
                }
            }
        }

        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }

        public void Send(Packet p)
        {
            Send(p.RawData);
        }

        public void Stop()
        {
            try
            {
                Logger.Log("Client iniating shutdown");
                Socket.Client.Shutdown(SocketShutdown.Both);
                Socket.Close();
                ShouldReceive = false;
                Join();
            }
            catch (Exception e)
            {
                Logger.Warn("Caught exception on close: " + e.Message);
            }
        }

        public void Join()
        {
            Thread.Join();
        }
    }
}
