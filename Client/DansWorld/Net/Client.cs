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
        private const bool PACKET_DEBUG = false;
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

                    byte[] buffer = new byte[2];
                    int readTotal = 0;
                    Stream.Read(buffer, 0, 2);
                    if (buffer[0] == 0)
                        continue;

                    byte length = buffer[0];
                    if (length < 2 && PACKET_DEBUG)
                    {
                        Logger.Error("Packet of length less than 2 received");
                        continue;
                    }

                    byte[] data = new byte[length];

                    while (readTotal < length)
                    {
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
                                }
                            }
                        }
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
                                }
                            }
                        }
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
                            _gameClient.ShowMessage(message, (p == null ? "" : p.Name));
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
                    else if (pkt.Family == PacketFamily.ENEMY)
                    {
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
                            enemy.Health = pkt.ReadInt();
                            enemy.SpriteID = pkt.ReadInt();
                            enemy.ServerID = pkt.ReadByte();
                            _gameClient.AddEnemy(enemy);
                        }
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
