using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using DansWorld.Common.Net;
using DansWorld.Common.IO;
using System.Collections.Generic;
using DansWorld.Server.GameEntities;
using DansWorld.Common.Enums;
using DansWorld.Server.Data;
using System.Security.Cryptography;

namespace DansWorld.Server
{
    public class Client
    {
        #region public accessors
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get { return Socket.GetStream(); } }
        public Thread Thread { get; private set; }
        public bool ShouldReceive { get; private set; }
        private Account _accountHandling;
        private Character _characterHandling;
        private int _id;
        #endregion

        #region private accessors
        private Server _server;
        private Database _database;
        #endregion

        public Client(Server server, TcpClient socket, int id, Database database)
        {
            _server = server;
            Socket = socket;
            Thread = new Thread(new ThreadStart(Main));
            ShouldReceive = true;
            _id = id;
            _database = database;
        }

        public void Main() 
        {
            Logger.Log(String.Format("Client handler thread spawned id:{0}", Thread.CurrentThread.ManagedThreadId));
            while (ShouldReceive) 
            {
                try
                {
                    byte[] buffer = new byte[2];
                    int readTotal = 0;
                    Stream.Read(buffer, 0, 2);
                    if (buffer[0] == 0)
                        return;

                    byte length = buffer[0];
                    if (length < 2)
                    {
                        Logger.Error("Packet of length less than 2 received");
                        return;
                    }

                    byte[] data = new byte[length];

                    while (readTotal != length)
                    {
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
                    Logger.Log("Packet length: " + length + " Packet data: " + sb.ToString());

                    PacketBuilder pb = new PacketBuilder().AddBytes(data);
                    Packet pkt = pb.Build();

                    try
                    {
                        if (pkt.Family == PacketFamily.LOGIN)
                        {
                            if (pkt.Action == PacketAction.REQUEST)
                            {
                                string user = pkt.ReadString(pkt.ReadByte());
                                string pass = pkt.ReadString(pkt.ReadByte());
                                pass = Utils.Security.GetHashString(pass + "PROCO304" + user);
                                CheckLogin(user, pass);
                            }
                        }
                        else if (pkt.Family == PacketFamily.REGISTER)
                        {
                            if (pkt.Action == PacketAction.REQUEST)
                            {
                                string username = pkt.ReadString(pkt.ReadByte());
                                string password = pkt.ReadString(pkt.ReadByte());
                                password = Utils.Security.GetHashString(password + "PROCO304" + username);
                                string email = pkt.ReadString(pkt.ReadByte());
                                string fullname = pkt.ReadString(pkt.ReadByte());
                                Logger.Log(String.Format("Requested username {0} password {1} email {2}", username, password, email));
                                bool userExists = false;
                                foreach (Account account in _server.Accounts)
                                {
                                    if (account.Username == username)
                                    {
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
                                int userid = pkt.ReadByte();
                                if (_accountHandling.Characters.Count >= userid) {
                                    pb = new PacketBuilder(PacketFamily.PLAY, PacketAction.ACCEPT);
                                    Character c = _accountHandling.GetCharacter(userid);
                                    _characterHandling = c;
                                    c.ServerID = _id;
                                    pb = pb.AddByte((byte)c.Name.Length).AddString(c.Name)
                                           .AddByte((byte)c.Level)
                                           .AddByte((byte)c.Gender)
                                           .AddInt(c.X)
                                           .AddInt(c.Y)
                                           .AddByte((byte)c.Facing)
                                           .AddInt(c.ServerID).AddInt(_server.LoggedInCharacters.Count);

                                    foreach (Character character in _server.LoggedInCharacters)
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

                                    _server.LoggedInCharacters.Add(c);

                                    foreach (Client client in _server.Clients)
                                    {
                                        if (client != this)
                                        {
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
                                }
                            }
                        }
                        else if (pkt.Family == PacketFamily.PLAYER)
                        {
                            if (pkt.Action == PacketAction.MOVE || pkt.Action == PacketAction.STOP)
                            {
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
                            else if (pkt.Action == PacketAction.LOGOUT && pkt.PeekInt() == _characterHandling.ServerID)
                            {
                                LogOut(_characterHandling);
                            }
                            else if (pkt.Action == PacketAction.TALK)
                            {
                                foreach (Client client in _server.Clients)
                                {
                                    client.Send(pkt);
                                }
                            }
                        }
                    } 
                    catch (Exception e)
                    {
                        Logger.Warn("Packet handling error. Remote endpoint: " 
                            + Socket.Client.RemoteEndPoint + "\n\r StackTrace:" + e.StackTrace + "\n\r Message:" + e.Message);
                    }

                }
                catch (Exception e)
                {
                    Logger.Error("Fatal Exception: " + e.Message);
                    Stop();
                    break;
                }
            }
        }
        
        private void CheckLogin(string user, string pass)
        {
            PacketBuilder pb = new PacketBuilder();
            Logger.Log(String.Format("Username: {0} Password: {1}", user, pass));
            bool foundUser = false;
            foreach (Account account in _server.Accounts)
            {
                if (account.Username == user)
                {
                    foundUser = true;
                    if (account.Password == pass)
                    {
                        if (account.State == AccountState.LoggedOut)
                        {
                            Logger.Log("Login accepted from " + Socket.Client.RemoteEndPoint + " for account " + user);
                            pb = new PacketBuilder()
                                .AddByte((byte)PacketFamily.LOGIN)
                                .AddByte((byte)PacketAction.ACCEPT);
                            foreach (Character character in account.Characters)
                            {
                                pb = pb.AddByte((byte)character.Name.Length)
                                    .AddString(character.Name)
                                    .AddByte((byte)character.Level)
                                    .AddByte((byte)character.Gender);
                            }
                            account.State = AccountState.LoggedIn;
                            _accountHandling = account;
                        }
                        else
                        {
                            pb = new PacketBuilder()
                                .AddByte((byte)PacketFamily.LOGIN)
                                .AddByte((byte)PacketAction.REJECT)
                                .AddString("Account Logged in somewhere else");
                        }
                    }
                    else
                    {
                        pb = new PacketBuilder()
                            .AddByte((byte)PacketFamily.LOGIN)
                            .AddByte((byte)PacketAction.REJECT)
                            .AddString("Invalid Password");
                    }
                }
            }

            if (!foundUser)
            {
                pb = new PacketBuilder()
                    .AddByte((byte)PacketFamily.LOGIN)
                    .AddByte((byte)PacketAction.REJECT)
                    .AddString("Invalid Username");
            }
            Send(pb.Build());
        }

        public void Start() 
        {
            if (!Thread.IsAlive)
                Thread.Start();
        }

        public void Send(byte[] data)
        {
            try
            {
                byte length = (byte)data.Length;
                Stream.Write(new byte[] { length, 0 }, 0, 2);
                Stream.Write(data, 0, length);
            }
            catch (Exception e)
            {
                Logger.Error("Fatal Exception: " + e.Message);
                Stop();
            }
        }

        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }

        public void Send(Packet packet)
        {
            Send(packet.RawData);
        }

        public void Join()
        {
            if (Thread.IsAlive)
                Thread.Join();
        }

        public void LogOut(Character character)
        {
            character.Save(_database);
            character.ServerID = 0;
            PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.LOGOUT);
            pb = pb.AddInt(character.ServerID);
            foreach (Client client in _server.Clients)
            {
                if (client.Socket.Connected)
                {
                    if (client != this)
                        client.Send(pb.Build());
                }
            }
        }

        public void Stop()
        {
            if (_accountHandling != null) _accountHandling.State = AccountState.LoggedOut;
            if (_server.LoggedInCharacters.Contains(_characterHandling)) _server.LoggedInCharacters.Remove(_characterHandling);
            if (_characterHandling != null) LogOut(_characterHandling);

            try
            {
                Logger.Log("Client iniating shutdown");
                if (_server.Clients.Contains(this))
                {
                    _server.Remove(this);
                }

                if (Socket != null && Socket.Connected)
                {
                    Socket.Client.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }

                if (Thread != null && Thread.IsAlive)
                {
                    ShouldReceive = false;
                    Thread.Join();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Caught exception on close: " + e.Message);
            }
        }
    }
}
