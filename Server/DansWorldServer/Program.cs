using System;

namespace DansWorld.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //main console entry point
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Dans World Server {0} ", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //starting the server on port 8081. Value picked arbitrarily 
            Server _server = new Server(8081);
            _server.Start();
            
        }
    }
}
