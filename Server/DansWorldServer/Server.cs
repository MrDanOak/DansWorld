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
        /// <summary>
        /// Listener class for incoming requested connections
        /// </summary>
        private TcpListener _listener { get; set; }
        /// <summary>
        /// The port for the server to be run on 
        /// </summary>
        private int _port { get; set; }
        /// <summary>
        /// The thread for the server to listen for incoming connections on
        /// </summary>
        private Thread _thread { get; set; }
        /// <summary>
        /// The currently connected network clients
        /// </summary>
        public List<Client> Clients { get; private set; }
        /// <summary>
        /// The enemies that exist within dans world
        /// </summary>
        public List<Enemy> Enemies;
        /// <summary>
        /// Whether the server should listen for incoming network connections. 
        /// Is set to false when server is in shutdown mode
        /// </summary>
        private bool _shouldListen { get; set; }
        /// <summary>
        /// The list of accounts that exist on the dans world server. 
        /// Loaded in from the MySQL connection when the server starts up.
        /// </summary>
        internal List<Account> Accounts { get; set; }
        /// <summary>
        /// The list of characters that are currently logged in
        /// </summary>
        internal List<PlayerCharacter> LoggedInPlayers { get; set; }
        /// <summary>
        /// Database connection to save accounts/characters
        /// </summary>
        private Database _database;
        /// <summary>
        /// a ping timer to test whether the connections are alive or not
        /// </summary>
        System.Timers.Timer _pingTimer;
        /// <summary>
        /// a timer, which has an OnTick event to randomly move the enemies within the game
        /// </summary>
        System.Timers.Timer _enemyUpdateTimer;
        /// <summary>
        /// Random object used to generate random values
        /// </summary>
        private Random _random = new Random();
        /// <summary>
        /// a timer, which saves the state of every connected character periodically
        /// </summary>
        System.Timers.Timer _databaseBackupTimer;

        /// <summary>
        /// Constructor for the server object
        /// </summary>
        /// <param name="port">The port which should be used for the server to bind itself to</param>
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

            //Loading the accounts into the server
            _LoadAccounts();
            //Loading the NPCs (enemies) into the server
            _LoadNPCs();

            Logger.Log(String.Format("Server object created {0}:{1}", "127.0.0.1", port));

            //we will ping every client every 10 seconds
            _pingTimer = new System.Timers.Timer(10000);
            _pingTimer.Elapsed += _pingTimer_Elapsed;
            _pingTimer.Start();

            //enemies should attempt to have their positions updated every .5 seconds
            _enemyUpdateTimer = new System.Timers.Timer(500);
            _enemyUpdateTimer.Elapsed += _enemyUpdateTimer_Elapsed;
            _enemyUpdateTimer.Start();

            //every 1min backup every character state
            _databaseBackupTimer = new System.Timers.Timer(60000);
            _databaseBackupTimer.Elapsed += _databaseBackupTimer_Elapsed;
            _databaseBackupTimer.Start();
        }

        private void _databaseBackupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (PlayerCharacter player in LoggedInPlayers)
            {
                player.Save(_database);
            }
        }

        private void _enemyUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Enemy enemy in Enemies)
            {
                //if the enemy moved
                if (enemy.IdleMove())
                {
                    PacketBuilder pb = new PacketBuilder(PacketFamily.ENEMY, PacketAction.MOVE);
                    pb = pb.AddInt(Enemies.IndexOf(enemy))
                            .AddInt(enemy.X)
                            .AddInt(enemy.Y)
                            .AddByte((byte)enemy.Facing);

                    //notify every client that the enemy has moved
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
            }
        }

        /// <summary>
        /// Used to load the NPCs into the server from the MySQL database
        /// </summary>
        private void _LoadNPCs()
        {
            //Building the query
            DataTable npcMapSpawnTable = _database.Select("*", "MapNPC");
            foreach (DataRow npcSpawnRow in npcMapSpawnTable.Rows)
            {
                DataTable NPCData = _database.Select("*", "NPC", "ID", Convert.ToInt32(npcSpawnRow["NPCID"]));
                DataRow NPCDataRow = NPCData.Rows[0];
                DataTable NPCIdentityTable = _database.Select("*", "NPCIdentity", "NPCName", NPCDataRow["NPCName"].ToString());
                DataRow NPCIDRow = NPCIdentityTable.Rows[0];
                for (int i = 0; i < Convert.ToInt32(npcSpawnRow["Quantity"]); i++)
                {
                    //building the new enemy object
                    Enemy enemy = new Enemy()
                    {
                        Name = NPCDataRow["NPCName"].ToString(),
                        Level = Convert.ToInt32(NPCDataRow["Level"]), 
                        Health = Convert.ToInt32(NPCDataRow["HP"]), 
                        MaxHealth = Convert.ToInt32(NPCDataRow["HP"]),
                        Strength = Convert.ToInt32(NPCDataRow["Strength"]), 
                        EXP = Convert.ToInt32(NPCDataRow["EXP"]), 
                        ID = Convert.ToInt32(NPCDataRow["ID"]),
                        SpawnX = Convert.ToInt32(npcSpawnRow["SpawnX"]) + RNG.Next(-300, 300),
                        SpawnY = Convert.ToInt32(npcSpawnRow["SpawnY"]) + RNG.Next(-300, 300),
                        X = Convert.ToInt32(npcSpawnRow["SpawnX"]) + RNG.Next(-300, 300), 
                        Y = Convert.ToInt32(npcSpawnRow["SpawnY"]) + RNG.Next(-300, 300),
                        SpriteID = Convert.ToInt32(NPCIDRow["NPCImage"])
                    };
                    //adding it to the enemies in the server
                    Enemies.Add(enemy);
                }
            }
            //display how many enemies we loaded
            Logger.Log(String.Format("Loaded {0} NPC(s)", Enemies.Count));
        }

        private void _LoadAccounts()
        {
            //building the query
            DataTable accountsTable = _database.Select("*", "Accounts");
            foreach (DataRow row in accountsTable.Rows)
            {
                //building the account object using the retrieved data
                Account account = new Account(row["Username"].ToString(), row["Password"].ToString(), row["Email"].ToString(), row["Fullname"].ToString());
                Accounts.Add(account);
            }

            int characters = 0;
            foreach (Account account in Accounts)
            {
                //building the new query based on the account name
                DataTable characterTable = _database.Select("*", "Characters", "AccountUsername", account.Username);
                foreach (DataRow row in characterTable.Rows)
                {
                    //building the characters that are owned by the account
                    PlayerCharacter character = new PlayerCharacter()
                    {
                        Name = row["CharacterName"].ToString(),
                        Gender = (Gender)Convert.ToInt32(row["Gender"].ToString()),
                        Level = Convert.ToInt32(row["Level"].ToString()),
                        X = Convert.ToInt32(row["X"].ToString()), 
                        Y = Convert.ToInt32(row["Y"].ToString())
                    };
                    //adding the characters to the account owner
                    account.Characters.Add(character);
                    characters++;
                }
            }
            //displaying to the console how many accounts and characters were loaded
            Logger.Log("Loaded " + Accounts.Count + " account(s)");
            Logger.Log("Loaded " + characters + " character(s)");
        }

        /// <summary>
        /// Ping timer event to test the status of every client
        /// </summary>
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
                    //if we're still waiting from 10s ago for the pong from the client 
                    if (client.NeedPong)
                    {
                        //we add it to the list to be terminated
                        disconnectedClients.Add(client);
                    }
                    else
                    {
                        //if we received the last pong that we need, we're going to send another ping
                        client.Send(pb.Build());
                        client.NeedPong = true;
                    }
                }
            }

            //removing every client that failed to check in with the server
            foreach (Client client in disconnectedClients)
            {
                client.Stop();
            }
        }
        
        /// <summary>
        /// Used to add a new client to the collection (Thread Safe)
        /// </summary>
        /// <param name="client">Client to add</param>
        public void Add(Client client)
        {
            lock (Clients)
            {
                if (!Clients.Contains(client))
                    Clients.Add(client);
            }
        }

        /// <summary>
        /// Used to remove a client from the collection (Thread Safe)
        /// </summary>
        /// <param name="client">Client to remove</param>
        public void Remove(Client client)
        {
            lock (Clients)
            {
                if (Clients.Contains(client))
                    Clients.Remove(client);
            }
        }

        /// <summary>
        /// Used to start the server on the given port
        /// </summary>
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

        public void Announce(string message)
        {
            PacketBuilder pb = new PacketBuilder(PacketFamily.SERVER, PacketAction.TALK);
            pb = pb.AddInt(message.Length)
                .AddString(message);
            foreach (Client client in Clients)
            {
                client.Send(pb.Build());
            }
        }

        /// <summary>
        /// Used to generate a random ID for the client
        /// Arbitrary limit of 10k
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Function that is used as the entry point for the listener thread
        /// </summary>
        public void Main() 
        {
            Logger.Log(String.Format("Server listener thread spawned id:{0}", Thread.CurrentThread.ManagedThreadId));
            while (_shouldListen) 
            {
                try 
                {
                    //building the new client object using the blocking function AcceptTcpClient. 
                    //the TcpClient is generated when an incomming connection is accepted.
                    Client client = new Client(this, _listener.AcceptTcpClient(), GenerateID(), _database);
                    Logger.Log(String.Format("Client connection accepted from {0} Assigned ID: {1}", client.Socket.Client.RemoteEndPoint, client.ID));
                    //adding the client to the collection
                    Add(client);
                    //starting the client so it listens for incoming data
                    client.Start();
                }
                catch (Exception e) 
                {
                    Logger.Error(e.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Joins any outside threads to this currently executing thread if an outside thread wishes to wait until this one terminates/
        /// </summary>
        public void Join() 
        {
            if (_thread.IsAlive)
                _thread.Join();
        }

        /// <summary>
        /// Shuts down the server and disposes of the thread object
        /// </summary>
        public void Shutdown() 
        {
            Logger.Log("Server shutting down...");
            _shouldListen = false;
            _thread.Join();
        }

    }
}
