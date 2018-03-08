using System;
using System.Text;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
    public class Server
    {
        private TcpListener _listener { get; set; }
        private int _port { get; set; }
        private Thread _thread { get; set; }
        public List<Client> Clients { get; private set; }
        private bool _shouldListen { get; set; }


        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _thread = new Thread(new ThreadStart(Main));
            Clients = new List<Client>();
            _shouldListen = true;
            _port = port;
            Logger.Log(String.Format("Server object created {0}:{1}", "127.0.0.1", port));
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
                    Client client = new Client(this, _listener.AcceptTcpClient(), Clients.Count);
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
            _thread.Abort();
            _thread.Join();
        }

    }
}
