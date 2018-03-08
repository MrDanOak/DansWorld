using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Dans World Server {0} ", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Server _server = new Server(8081);
            _server.Start();
            
        }
    }
}
