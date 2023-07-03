using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ExtensionFinder
{
    internal class Program
    {
        private static string line = "--------------------------------------------------------------------------------------";
        private static void ShowHelp()
        {
            Console.WriteLine("\n" + line);
            Console.WriteLine("Usage: ExtensionFinder [path]\n");
            Console.WriteLine("Arguments:");
            Console.WriteLine("  [path]  Path from which scan is started (e.g., C:/ , C:\\Folder1\\Folder2\\Folder3)");
            Console.WriteLine(" --help   Show help");
            Console.WriteLine(" --all    Scan all drives");
            Console.WriteLine(line + "\n");
        }
        private static void Scan(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine("\n" + line);
                Console.WriteLine("Scan started from " + path);
                Console.WriteLine("It might take some time!\n");
                var start = DateTime.Now;

                FileHelper.ScanDir(path);

                Console.WriteLine("\nScan finished!");
                #region elapsed time builder
                var duration = DateTime.Now - start;
                StringBuilder sb = new StringBuilder();
                if (duration.Days > 0)
                    sb.Append(duration.Days + "d");
                if (duration.Hours > 0)
                    sb.Append(duration.Hours + "h");
                if (duration.Minutes > 0)
                    sb.Append(duration.Minutes + "m");
                if (duration.Seconds > 0)
                    sb.Append(duration.Seconds + "s");
                if (duration.Milliseconds > 0)
                    sb.Append(duration.Milliseconds + "ms");
                #endregion
                Console.WriteLine("\nScan took " + sb.ToString());
            }
            else
            {
                Console.WriteLine("No such directory as " + path);
            }
        }

        static void Main(string[] args)
        {
            
            //No args - Help
            if (args.Length == 0)
            {
                ShowHelp();
            }

            //One argument
            else if (args.Length == 1)
            {
                //Help
                if (args[0] == "--help")
                {
                    ShowHelp();
                }
                else if (args[0] == "--all")
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo drive in allDrives)
                    {
                        Scan(drive.Name);
                    }
                }
                //Scan by path
                else
                {
                    Scan(args[0]);
                }
            }
        }



    }
}
