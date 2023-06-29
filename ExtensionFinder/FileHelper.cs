using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace ExtensionFinder
{
    public static class FileHelper
    {
        private static char mirror_malware = '\u202e';
        public static List<string> GetFiles(string directory)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {

                    files.Add(file.Replace(directory + @"\", ""));
                }
            }
            //На випадок якшо папка вимагає адмінського доступу
            catch { }


            return files;
        }

        public static List<string> GetDirs(string directory)
        {
            List<string> dirs = new List<string>();

            try
            {
                foreach (string dir in Directory.GetDirectories(directory))
                {
                    dirs.Add(dir);
                }
            }
            catch { }
            return dirs;
        }

        //Main logic
        private static void ScanDirHelper(string directory,
            ref int scanedFiles, ref int scanedDirs, ref int foundFiles, ref int foundDirs)
        {

            List<string> dirs = GetDirs(directory);

            foreach (string dir in dirs)
            {
                scanedDirs++;

                List<string> files = GetFiles(dir);

                //Check if folder contains any mirrored files or has keyword files
                bool hasKeyWordFiles = files.Any(file => file.Contains(mirror_malware));


                if (hasKeyWordFiles)
                {

                    foundDirs++;
                    Console.Write("\n" + dir + "\n");

                    foreach (string file in files)
                    {
                        scanedFiles++;

                        if (file.Contains(mirror_malware))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            foundFiles++;
                            Console.WriteLine("\t" + file);
                        }
                        Console.ResetColor();

                    }
                }

                if (GetDirs(dir).Count > 0)
                    ScanDirHelper(dir, ref scanedFiles, ref scanedDirs, ref foundFiles, ref foundDirs);
            }

        }

        public static void ScanDir(string directory)
        {
            if (!Directory.Exists(directory)) return;

            int scanedFiles = 0;
            int scanedDirs = 0;

            int foundFiles = 0;
            int foundDirs = 0;

            scanedDirs++;

            List<string> files = GetFiles(directory);

            bool hasKeyWordFiles = files.Any(file => file.Contains(mirror_malware));

            if (hasKeyWordFiles)
            {
                foundDirs++;
                Console.Write("\n" + directory + "\n");

                foreach (string file in files)
                {
                    scanedFiles++;

                    if (file.Contains(mirror_malware))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        foundFiles++;
                        Console.WriteLine("\t" + file);
                    }
                    Console.ResetColor();
                }
            }


            //Scan all other folders
            ScanDirHelper(directory, ref scanedFiles, ref scanedDirs, ref foundFiles, ref foundDirs);
            Console.Write(
             "\n\nScaned dirs : " + scanedDirs +
               "\nScaned files: " + scanedFiles +
               "\n\nDirs with malware files : " + foundDirs +
               "\nMalware files: " + foundFiles);


        }
    }
}
