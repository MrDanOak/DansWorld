using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using DansWorld.Common.IO;

namespace DansWorld.GameClient.Net
{
    class Client
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get { return Socket.GetStream(); } }
        public bool Connected { get { return Socket.Connected; } }
        public bool ShouldReceive { get; private set; }
        public Thread Thread { get; private set; }

        public Client(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            this.ShouldReceive = true;
            this.Socket = new TcpClient();
            this.Thread = new Thread(new ThreadStart(Main));
        }

        public void Connect()
        {
            try
            {
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
                        Console.WriteLine("Packet of length less than 2 received");
                        return;
                    }

                    Console.WriteLine("Received packet of length: {0}", length);
                    byte[] data = new byte[length];

                    while (readTotal != length)
                    {
                        int read = Stream.Read(data, 0, length);
                        if (read == 0)
                        {
                            Console.WriteLine("Packet seems empty..");
                            return;
                        }
                        readTotal += read;
                    }

                    Console.Write("Packet data: ");
                    for (int i = 0; i < data.Length; i++) { Console.Write("{" + data[i] + "} "); }
                    Console.WriteLine();
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
            byte length = (byte)data.Length;
            Stream.Write(new byte[] { length, 0 }, 0, length);
            Stream.Write(data, 0, length);
        }

        public void Send(string data)
        {
            Send(Encoding.ASCII.GetBytes(data));
        }

        public void Stop()
        {
            try
            {
                Logger.Log("Client iniating shutdown");
                this.Socket.Client.Shutdown(SocketShutdown.Both);
                this.Socket.Close();
                this.ShouldReceive = false;
                this.Thread.Abort();
                this.Join();
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
