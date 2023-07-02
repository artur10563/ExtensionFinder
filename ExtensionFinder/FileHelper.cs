using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace ExtensionFinder
{
    public static class FileHelper
    {
        private static char mirror_malware = '\u202e';
        private static List<string> malware_extensions = new List<string>
        {".src" , ".pif", ".docm", ".pptm", ".xlsm", ".cpl", ".crt", ".ins", ".isp" };

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

        //Check if file has multiple extensions
        private static bool MultipleExtensions(string file, out int count)
        {
            count = 0;
            string extension;

            do
            {
                extension = Path.GetExtension(file);
                if (extension.Length != 0)
                {
                    count++;
                    file = file.Remove(file.Length - extension.Length, extension.Length);
                }
            } while (!string.IsNullOrEmpty(extension));

            return count > 1;
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

                //Check if folder contains any mirrored files or has keyword files or >1 extension files
                bool hasKeyWordFiles =
                    files.Any(file => file.Contains(mirror_malware) ||
                                      malware_extensions.Any(me => file.EndsWith(me)) ||
                                      MultipleExtensions(file, out _)
                                      );

                if (hasKeyWordFiles)
                {

                    foundDirs++;
                    Console.Write("\n" + dir + "\n");

                    foreach (string file in files)
                    {

                        scanedFiles++;

                        bool contains_malware_extension = malware_extensions.Any(me => file.EndsWith(me));

                        if (file.Contains(mirror_malware))
                        {
                            foundFiles++;
                            Logger.CW("\t" + file, ConsoleColor.Red);
                            Logger.Log(dir + @"\" + file, "u202e");
                        }

                        else if (contains_malware_extension)
                        {
                            foundFiles++;
                            Logger.CW("\t" + file, ConsoleColor.Yellow);
                            Logger.Log(file, "extension");
                        }

                        //High chance for false-positive results
                        else if (MultipleExtensions(file, out int count))
                        {
                            foundFiles++;
                            Logger.CW("\t" + file, ConsoleColor.DarkYellow);
                            Logger.Log(file, "multiple extensions");

                        }

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

            bool hasKeyWordFiles = files.Any(file => file.Contains(mirror_malware) || malware_extensions.Any(me => file.EndsWith(me)));

            if (hasKeyWordFiles)
            {
                foundDirs++;
                Console.Write("\n" + directory + "\n");

                foreach (string file in files)
                {
                    scanedFiles++;
                    bool contains_malware_extension = malware_extensions.Any(me => file.EndsWith(me));
                    if (file.Contains(mirror_malware) || contains_malware_extension)
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
