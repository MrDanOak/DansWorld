using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using DansWorld.Common.Net;
using DansWorld.Common.IO;
using System.Collections.Generic;
using DansWorld.Server.GameEntities;

namespace DansWorld.Server
{
    public class Client
    {
        #region public accessors
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get { return Socket.GetStream(); } }
        public Thread Thread { get; private set; }
        public bool ShouldReceive { get; private set; }
        #endregion

        #region private accessors
        private Server _server;
        #endregion

        public Client(Server server, TcpClient socket, int id)
        {
            _server = server;
            Socket = socket;
            Thread = new Thread(new ThreadStart(Main));
            ShouldReceive = true;
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

                    if (pkt.Family == PacketFamily.Login)
                    {
                        if (pkt.Action == PacketAction.Request)
                        {
                            Logger.Log("Login Requested from accepted packet");
                            string s = pkt.ReadString(pkt.Length - 2);
                            string user = s.Split("p:")[0];
                            user = user.Substring(2, user.Length - 2);
                            string pass = s.Split("p:")[1];
                            CheckLogin(user, pass);
                        }
                    }

                }
                catch (Exception e)
                {
                    Logger.Error("Fatal Exception: " + e.Message);
                    break;
                }
            }
        }
        
        private void CheckLogin(string user, string pass)
        {
            PacketBuilder pb = new PacketBuilder();
            Logger.Log(String.Format("Username: {0} Password: {1}", user, pass));
            bool foundUser = false;
            foreach (Account account in _server._accounts)
            {
                if (account.Username == user)
                {
                    foundUser = true;
                    if (account.Password == pass)
                    {
                        Logger.Log("Login accepted from " + Socket.Client.RemoteEndPoint + " for account " + user);
                        pb = new PacketBuilder()
                            .AddByte((byte)PacketFamily.Login)
                            .AddByte((byte)PacketAction.Accept);
                        foreach (Character character in account.Characters)
                        {
                            pb = pb.AddByte((byte)character.Name.Length)
                                .AddString(character.Name)
                                .AddByte((byte)character.Level)
                                .AddByte((byte)character.Gender);
                            Logger.Log("Building serialised packet of character: " + character.Name);
                        }
                    }
                    else
                    {
                        pb = new PacketBuilder()
                            .AddByte((byte)PacketFamily.Login)
                            .AddByte((byte)PacketAction.Reject)
                            .AddString("Invalid Password");
                    }
                }
            }

            if (!foundUser)
            {
                pb = new PacketBuilder()
                    .AddByte((byte)PacketFamily.Login)
                    .AddByte((byte)PacketAction.Reject)
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

        public void Stop()
        {
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
