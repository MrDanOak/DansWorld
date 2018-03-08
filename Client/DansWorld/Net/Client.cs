using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using DansWorld.Common.IO;
using DansWorld.Common.Net;
using DansWorld.Common.GameEntities;
using DansWorld.Common.Enums;

namespace DansWorld.GameClient.Net
{
    public class Client
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get { if (Socket.Connected) return Socket.GetStream(); else return null; } }
        public bool Connected { get { return Socket.Connected; } }
        public bool ShouldReceive { get; private set; }
        public Thread Thread { get; private set; }
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
                Console.WriteLine(e.Message);
            }
        }

        public void Main()
        {
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
                        Logger.Warn("Packet of length less than 2 received");
                        return;
                    }

                    Logger.Log(String.Format("Received packet of length: {0}", length));
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

                    Console.Write("Packet data: ");
                    for (int i = 0; i < data.Length; i++) { Console.Write("{" + data[i] + "} "); }
                    Console.WriteLine();

                    PacketBuilder pb = new PacketBuilder().AddBytes(data);
                    Packet pkt = pb.Build();

                    if (pkt.Family == PacketFamily.LOGIN)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            _gameClient.DisplayMessage("Login accepted");
                            _gameClient.SetState(GameExecution.GameState.LoggedIn);
                            while (pkt.ReadPosition < pkt.RawData.Length)
                            {
                                int nameLength = pkt.ReadByte();
                                if (nameLength > 0)
                                {
                                    Character c = new Character()
                                    {
                                        Name = pkt.ReadString(nameLength),
                                        Level = pkt.ReadByte(),
                                        Gender = (Gender)pkt.ReadByte()
                                    };
                                    _gameClient.AddCharacter(c);
                                }
                            }
                        }
                        else
                        {
                            _gameClient.DisplayMessage(pkt.ReadString(pkt.Length - 2));
                        }
                    }
                    else if (pkt.Family == PacketFamily.REGISTER)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            _gameClient.SetState(GameExecution.GameState.MainMenu);
                            _gameClient.DisplayMessage("Account created! Username: " + pkt.ReadString(pkt.ReadByte()));
                        }
                        else
                        {
                            _gameClient.DisplayMessage(pkt.ReadString(pkt.ReadByte()));
                        }
                    }
                    else if (pkt.Family == PacketFamily.PLAY)
                    {
                        if (pkt.Action == PacketAction.ACCEPT)
                        {
                            string charName = pkt.ReadString(pkt.ReadByte());
                            int level = pkt.ReadByte();
                            _gameClient.SetState(GameExecution.GameState.Playing);
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

        public void Send(byte[] data)
        {
            int tries = 0;
            bool success = false;

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
                Thread.Abort();
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
