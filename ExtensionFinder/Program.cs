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
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: ExtensionFinder [path]\n");
            Console.WriteLine("Arguments:");
            Console.WriteLine("  [path]  Path from which scan is started (e.g., C:/ , C:\\Folder1\\Folder2\\Folder3)");
        }
        static void Main(string[] args)
        {
            ;

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
                //Check if such path exists
                else
                {
                    string path = args[0];
                    if (Directory.Exists(path))
                    {
                        var start = DateTime.Now;
                        Console.WriteLine("Scan started from " + path);
                        Console.WriteLine("It might take some time!\n");

                        FileHelper.ScanDir(path);

                        Console.WriteLine("\nScan finished!");
                        var duration = DateTime.Now - start;
                        StringBuilder sb = new StringBuilder();
                        //через стрінг білдер динамічно робити шоб добавляти тільки якшо більше нуля
                        string formattedDuration = string.Format("{0}d{1}h{2}m{3}s{4}ms",
                            duration.Days, duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);

                        Console.WriteLine("\nScan took " + formattedDuration);


                    }
                    else
                    {
                        Console.WriteLine("No such directory as " + path);
                    }
                }


            }

        }
    }
}
