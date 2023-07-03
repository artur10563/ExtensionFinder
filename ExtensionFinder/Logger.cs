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
            string logHeader = "\n*[" + DateTime.Now.ToString("G") + "]\n";

            if (File.Exists(logPath))
            {
                //Read logs
                StringBuilder firstLog = new StringBuilder();
                StringBuilder secondLog = new StringBuilder();

                using (StreamReader reader = new StreamReader(logPath))
                {
                    string line;
                    bool firstLogStarted = false;
                    bool secondLogStarted = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        line += "\n";

                        //Separate logs
                        if (line.StartsWith("*"))
                        {
                            if (secondLogStarted) break;

                            if (!firstLogStarted)
                            {
                                firstLogStarted = true;
                            }

                            else if (firstLogStarted)
                            {
                                secondLogStarted = true;
                            }

                        }

                        //Save first log
                        if (firstLogStarted && !secondLogStarted)
                            firstLog.Append(line);

                        //Save second log
                        if (secondLogStarted)
                            secondLog.Append(line);
                    }


                }

                if (secondLog.Length == 0)
                    secondLog = firstLog;

                //Save last log
                File.WriteAllText(logPath, secondLog.ToString());

                //Set start for new log
                File.AppendAllText(logPath, logHeader);


            }
            else
            {
                //Set start for new log
                File.WriteAllText(logPath, logHeader);
            }

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
