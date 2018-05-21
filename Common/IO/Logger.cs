using System;
using System.Threading;
using System.IO;
using DansWorld.Common.Enums;

namespace DansWorld.Common.IO
{
    /// <summary>
    /// debugging output utility class
    /// </summary>
    public class Logger
    {
        //write to file or not
        public static bool WriteToFile = false;
        //builds directory based off the given string
        public static string Path = Directory.GetCurrentDirectory() + "\\Logs\\DW.log";
        //write lock for cross thread calls
        static ReaderWriterLockSlim writeLock = new ReaderWriterLockSlim();

        public static void Log(string log, LogLevel level = LogLevel.INFO)
        {
            DateTime dt = DateTime.Now;
            //sets the colour of the text based on the level of the log
            Console.ForegroundColor = (level == LogLevel.ERROR ? ConsoleColor.Red : level == LogLevel.WARN ? ConsoleColor.Yellow : ConsoleColor.Cyan);
            Console.Write("[{0:T}]", dt);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[{0}] {1}", level, log);
            if (WriteToFile)
            {
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }

                if (!File.Exists(Path))
                {
                    File.Create(Path);
                }

                File.SetAttributes(Path, FileAttributes.Normal);

                writeLock.EnterWriteLock();
                try
                {
                    StreamWriter sw = File.AppendText(Path);
                    sw.WriteLine(String.Format("[{0:T}][{1}] {2}", dt, level, log));
                    sw.Close();
                }
                catch (Exception)
                {

                }
                finally
                {
                    writeLock.ExitWriteLock();
                }
            }

        }

        public static void Warn(string message)
        {
            Log(message, LogLevel.WARN);
        }

        public static void Error(string message)
        {
            Log(message, LogLevel.ERROR);
        }
    }
}
