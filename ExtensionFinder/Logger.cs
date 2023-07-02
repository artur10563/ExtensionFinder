using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionFinder
{
    //Save only previous and current log
    internal static class Logger
    {
        private static string logPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ExtensionScanLog.txt";


        static Logger()
        {
        }


        internal static void Log(string message, string malware_type)
        {
            string currentTime = DateTime.Now.ToString("HH:mm");
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(currentTime);
            sb.Append("] ");
            sb.Append("[");
            sb.Append(malware_type);
            sb.Append("]: ");
            sb.Append(message + "\n");

            File.AppendAllText(logPath, sb.ToString());
        }

        internal static void CW(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }
}
