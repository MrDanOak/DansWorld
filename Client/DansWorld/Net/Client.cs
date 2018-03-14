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
using System.Globalization;

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
        private int pingFromServer = 0;

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
                            Character character = new Character()
                            {
                                Name = pkt.ReadString(pkt.ReadByte()),
                                Level = pkt.ReadByte(),
                                Gender = (Gender)pkt.ReadByte(),
                                X = pkt.ReadInt(), 
                                Y = pkt.ReadInt(), 
                                Facing = (Direction)pkt.ReadByte(), 
                                ServerID = pkt.ReadInt()
                            };
                            _gameClient.ClearCharacters();
                            _gameClient.SetState(GameExecution.GameState.Playing);
                            _gameClient.CharacterID = character.ServerID;
                            _gameClient.AddCharacter(character);
                            int loggedInCharacters = pkt.ReadInt();
                            for (int i = 0; i < loggedInCharacters; i++)
                            {
                                Character c = new Character()
                                {
                                    Name = pkt.ReadString(pkt.ReadByte()),
                                    Level = pkt.ReadByte(),
                                    Gender = (Gender)pkt.ReadByte(),
                                    X = pkt.ReadInt(),
                                    Y = pkt.ReadInt(),
                                    Facing = (Direction)pkt.ReadByte(),
                                    ServerID = pkt.ReadInt()
                                };
                                _gameClient.AddCharacter(c);
                            }
                        }
                    }
                    else if (pkt.Family == PacketFamily.PLAYER)
                    {
                        if (pkt.Action == PacketAction.WELCOME)
                        {
                            Character character = new Character()
                            {
                                Name = pkt.ReadString(pkt.ReadByte()),
                                Level = pkt.ReadByte(),
                                Gender = (Gender)pkt.ReadByte(),
                                X = pkt.ReadInt(), 
                                Y = pkt.ReadInt(),
                                Facing = (Direction)pkt.ReadByte(),
                                ServerID = pkt.ReadInt()
                            };
                            _gameClient.AddCharacter(character);
                        }
                        else if (pkt.Action == PacketAction.MOVE)
                        {
                            int x = pkt.ReadInt();
                            int y = pkt.ReadInt();
                            Direction d = (Direction)pkt.ReadByte();
                            int id = pkt.ReadInt();
                            foreach (Character character in _gameClient.GetCharacters())
                            {
                                if (character.ServerID == id && _gameClient.CharacterID != id)
                                {
                                    character.X = x;
                                    character.Y = y;
                                    character.IsWalking = true;
                                    character.IsIdle = false;
                                    character.SetFacing(d);
                                }
                            }
                        }
                        else if (pkt.Action == PacketAction.STOP)
                        {
                            int x = pkt.ReadInt();
                            int y = pkt.ReadInt();
                            Direction d = (Direction)pkt.ReadByte();
                            int id = pkt.ReadInt();
                            foreach (Character character in _gameClient.GetCharacters())
                            {
                                if (character.ServerID == id && _gameClient.CharacterID != id)
                                {
                                    character.X = x;
                                    character.Y = y;
                                    character.IsWalking = false;
                                    character.IsIdle = true;
                                    character.SetFacing(d);
                                }
                            }
                        }
                        else if (pkt.Action == PacketAction.LOGOUT)
                        {
                            Character toRemove = null;
                            foreach (Character character in _gameClient.GetCharacters())
                            {
                                if (character.ServerID == pkt.PeekInt())
                                    toRemove = character;
                            }
                            if (toRemove != null)
                                _gameClient.RemoveCharacter(toRemove);
                        }
                        else if (pkt.Action == PacketAction.TALK)
                        {
                            Character character = null;
                            string message = pkt.ReadString(pkt.ReadInt());
                            int id = pkt.ReadInt();
                            foreach (Character c in _gameClient.GetCharacters())
                            {
                                if (c.ServerID == id)
                                {
                                    character = c;
                                }
                            }
                            _gameClient.ShowMessage(message, (character == null ? "" : character.Name));
                        }
                    }
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
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message + " Stack " + e.StackTrace);
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
