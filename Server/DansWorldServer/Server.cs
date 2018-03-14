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

namespace DansWorld.Server
{
    public class Server
    {
        private TcpListener _listener { get; set; }
        private int _port { get; set; }
        private Thread _thread { get; set; }
        public List<Client> Clients { get; private set; }
        private bool _shouldListen { get; set; }
        internal List<Account> Accounts { get; set; }
        internal List<PlayerCharacter> LoggedInPlayers { get; set; }
        private Database _database;
        System.Timers.Timer _pingTimer;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _thread = new Thread(new ThreadStart(Main));
            Clients = new List<Client>();
            Accounts = new List<Account>();
            _shouldListen = true;
            _port = port;
            LoggedInPlayers = new List<PlayerCharacter>();
            _database = new Database();
            _LoadAccounts();
            Logger.Log(String.Format("Server object created {0}:{1}", "127.0.0.1", port));

            _pingTimer = new System.Timers.Timer(1000);
            _pingTimer.Elapsed += _pingTimer_Elapsed;
            _pingTimer.Start();
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

            lock (Clients)
            {
                foreach (Client client in Clients)
                {
                    client.Send(pb.Build());
                }
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

        public void Main() 
        {
            Logger.Log(String.Format("Server listener thread spawned id:{0}", Thread.CurrentThread.ManagedThreadId));
            while (_shouldListen) 
            {
                try 
                {
                    Client client = new Client(this, _listener.AcceptTcpClient(), Clients.Count, _database);
                    Logger.Log(String.Format("Client connection accepted from {0}", client.Socket.Client.RemoteEndPoint));
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
