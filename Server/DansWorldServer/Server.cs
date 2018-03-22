using System;
using System.Text;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using DansWorld.Common.IO;
using DansWorld.Server.GameEntities;
using DansWorld.Server.Data;
using System.Data;
using DansWorld.Common.Enums;
using DansWorld.Common.Net;
using DansWorld.Server.Utils;

namespace DansWorld.Server
{
    public class Server
    {
        private TcpListener _listener { get; set; }
        private int _port { get; set; }
        private Thread _thread { get; set; }
        public List<Client> Clients { get; private set; }
        public List<Enemy> Enemies;
        private bool _shouldListen { get; set; }
        internal List<Account> Accounts { get; set; }
        internal List<PlayerCharacter> LoggedInPlayers { get; set; }
        private Database _database;
        System.Timers.Timer _pingTimer;
        System.Timers.Timer _enemyUpdateTimer;
        private Random _random = new Random();

        public Server(int port)
        {
            Clients = new List<Client>();
            LoggedInPlayers = new List<PlayerCharacter>();
            Enemies = new List<Enemy>();
            Accounts = new List<Account>();

            _listener = new TcpListener(IPAddress.Any, port);
            _thread = new Thread(new ThreadStart(Main));
            _shouldListen = true;
            _port = port;
            _database = new Database();

            _LoadAccounts();
            _LoadNPCs();

            Logger.Log(String.Format("Server object created {0}:{1}", "127.0.0.1", port));

            _pingTimer = new System.Timers.Timer(10000);
            _pingTimer.Elapsed += _pingTimer_Elapsed;
            _pingTimer.Start();


            _enemyUpdateTimer = new System.Timers.Timer(500);
            _enemyUpdateTimer.Elapsed += _enemyUpdateTimer_Elapsed;
            _enemyUpdateTimer.Start();
        }

        private void _enemyUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int servID = 0;
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.IdleMove())
                {
                    PacketBuilder pb = new PacketBuilder(PacketFamily.ENEMY, PacketAction.MOVE);
                    pb = pb.AddInt(servID)
                            .AddInt(enemy.X)
                            .AddInt(enemy.Y)
                            .AddByte((byte)enemy.Facing);

                    lock (Clients)
                    {
                        foreach (Client client in Clients)
                        {
                            if (client.Account != null && client.Account.State == AccountState.Playing)
                            {
                                client.Send(pb.Build());
                            }
                        }
                    }
                }
                servID += 1;
            }
        }

        private void _LoadNPCs()
        {
            DataTable npcMapSpawnTable = _database.Select("*", "MapNPC");
            foreach (DataRow npcSpawnRow in npcMapSpawnTable.Rows)
            {
                DataTable NPCData = _database.Select("*", "NPC", "ID", Convert.ToInt32(npcSpawnRow["NPCID"]));
                DataRow NPCDataRow = NPCData.Rows[0];
                DataTable NPCIdentityTable = _database.Select("*", "NPCIdentity", "NPCName", NPCDataRow["NPCName"].ToString());
                DataRow NPCIDRow = NPCIdentityTable.Rows[0];
                for (int i = 0; i < Convert.ToInt32(npcSpawnRow["Quantity"]); i++)
                {
                    Enemy enemy = new Enemy()
                    {
                        Name = NPCDataRow["NPCName"].ToString(),
                        Level = Convert.ToInt32(NPCDataRow["Level"]), 
                        Health = Convert.ToInt32(NPCDataRow["HP"]), 
                        Strength = Convert.ToInt32(NPCDataRow["Strength"]), 
                        EXP = Convert.ToInt32(NPCDataRow["EXP"]), 
                        ID = Convert.ToInt32(NPCDataRow["ID"]), 
                        X = Convert.ToInt32(npcSpawnRow["SpawnX"]), 
                        Y = Convert.ToInt32(npcSpawnRow["SpawnY"]),
                        SpriteID = Convert.ToInt32(NPCIDRow["NPCImage"])
                    };
                    Enemies.Add(enemy);
                }
            }
            Logger.Log(String.Format("Loaded {0} NPC(s)", Enemies.Count));
        }

        private void _LoadAccounts()
        {
            DataTable accountsTable = _database.Select("*", "Accounts");
            foreach (DataRow row in accountsTable.Rows)
            {
                Account account = new Account(row["Username"].ToString(), row["Password"].ToString(), row["Email"].ToString(), row["Fullname"].ToString());
                Accounts.Add(account);
            }

            int characters = 0;
            foreach (Account account in Accounts)
            {
                DataTable characterTable = _database.Select("*", "Characters", "AccountUsername", account.Username);
                foreach (DataRow row in characterTable.Rows)
                {
                    PlayerCharacter character = new PlayerCharacter()
                    {
                        Name = row["CharacterName"].ToString(),
                        Gender = (Gender)Convert.ToInt32(row["Gender"].ToString()),
                        Level = Convert.ToInt32(row["Level"].ToString()),
                        X = Convert.ToInt32(row["X"].ToString()), 
                        Y = Convert.ToInt32(row["Y"].ToString())
                    };
                    account.Characters.Add(character);
                    characters++;
                }
            }
            Logger.Log("Loaded " + Accounts.Count + " account(s)");
            Logger.Log("Loaded " + characters + " character(s)");
        }

        private void _pingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PacketBuilder pb = new PacketBuilder(PacketFamily.CONNECTION, PacketAction.PING);
            DateTime now = DateTime.Now.ToUniversalTime();
            string nowString = now.ToString("hh.mm.ss.ffffff");
            pb = pb.AddInt(nowString.Length).AddString(nowString);

            List<Client> disconnectedClients = new List<Client>();

            lock (Clients)
            {
                foreach (Client client in Clients)
                {
                    if (client.NeedPong)
                    {
                        disconnectedClients.Add(client);
                    }
                    else
                    {
                        client.Send(pb.Build());
                        client.NeedPong = true;
                    }
                }
            }

            foreach (Client client in disconnectedClients)
            {
                client.Stop();
            }
        }

        public void Add(Client client)
        {
            lock (Clients)
            {
                if (!Clients.Contains(client))
                    Clients.Add(client);
            }
        }

        public void Remove(Client client)
        {
            lock (Clients)
            {
                if (Clients.Contains(client))
                    Clients.Remove(client);
            }
        }

        public void Start() 
        {
            try
            {
                _listener.Start(100);
                Logger.Log("Listening 127.0.0.1:" + _port);
                _thread.Start();
            }
            catch (Exception e) 
            {
                Logger.Error("Couldn't start listener... Do you have root permissions? Error:" + e.Message);
            }
        }

        public int GenerateID()
        {
            bool IDInUse;
            int i;
            do
            {
                i = RNG.Next(0, 10000);
                IDInUse = false;
                foreach (Client client in Clients)
                {
                    if (client.ID == i) IDInUse = true;
                }
            } while (IDInUse);
            return i;
        }

        public void Main() 
        {
            Logger.Log(String.Format("Server listener thread spawned id:{0}", Thread.CurrentThread.ManagedThreadId));
            while (_shouldListen) 
            {
                try 
                {
                    Client client = new Client(this, _listener.AcceptTcpClient(), GenerateID(), _database);
                    Logger.Log(String.Format("Client connection accepted from {0} Assigned ID: {1}", client.Socket.Client.RemoteEndPoint, client.ID));
                    Add(client);
                    client.Start();
                }
                catch (Exception e) 
                {
                    Logger.Error(e.Message.ToString());
                }
            }
        }

        public void Join() 
        {
            if (_thread.IsAlive)
                _thread.Join();
        }

        public void Shutdown() 
        {
            Logger.Log("Server shutting down...");
            _shouldListen = false;
            _thread.Join();
        }

    }
}
