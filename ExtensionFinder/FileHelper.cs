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
        private static List<string> commonExtensions = new List<string>
{
    // Text Documents
    ".txt", ".doc", ".docx", ".rtf", ".odt", ".wpd",
    
    // Spreadsheets
    ".xls", ".xlsx", ".csv", ".ods",
    
    // Presentations
    ".ppt", ".pptx", ".pps", ".ppsx", ".odp",
    
    // PDF
    ".pdf",
    
    // Images
    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg", ".ico",
    
    // Audio
    ".mp3", ".wav", ".wma", ".aac", ".flac", ".ogg",
    
    // Video
    ".mp4", ".avi", ".mkv", ".wmv", ".mov", ".flv", ".m4v", ".mpeg", ".3gp", ".webm",
    
    // Compressed Archives
    ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz",
    
    // Programming Files
    ".cpp", ".cs", ".java", ".py", ".rb", ".php", ".html", ".css", ".js", ".xml", ".json",
    
    // Executable Files
    ".exe", ".dll", ".bat", ".com", ".app", ".msi",
    
    // Database Files
    ".mdb", ".accdb", ".sql", ".db", ".dbf", ".sqlite", ".backup",
    
    // Fonts
    ".ttf", ".otf", ".woff", ".woff2",
    
    // Configuration Files
    ".ini", ".cfg", ".yaml", ".toml",
    
    // System Files
    ".bak", ".tmp", ".log",
    
    // Documents
    ".odt", ".ott", ".fodt", ".docm", ".dot", ".dotm", ".dotx",
    
    // Spreadsheets
    ".ods", ".ots", ".fods", ".xlsm", ".xlt", ".xltm", ".xltx",
    
    // Presentations
    ".odp", ".otp", ".fodp", ".pptm", ".pot", ".potm", ".potx",
    
    // Emails
    ".eml", ".msg", ".pst", ".mbox",
    
    // Web Files
    ".html", ".htm", ".php", ".css", ".js", ".asp", ".aspx", ".jsp", ".php", ".xhtml",
    
    // Certificates
    ".crt", ".pem", ".pfx",
    
    // Virtual Machine Files
    ".vmdk", ".vhd", ".vhdx", ".ova", ".ovf",
    
    // Backup Files
    ".bak", ".zip", ".tar", ".gz", ".bz2", ".7z",
    
    // Configuration Files
    ".ini", ".cfg", ".conf", ".yaml", ".xml",
    
    // Miscellaneous
    ".dat", ".bin", ".log", ".cache", ".tmp", ".bak",
    
    // Additional Extensions (to reach 300)
    ".src", ".pif", ".cpl", ".ins", ".isp", ".csv", ".dbf", ".dbx", ".eml", ".eps", ".fla", ".fnt",
    ".hlp", ".ics", ".iso", ".lnk", ".lst", ".man", ".mht", ".mhtml", ".ods", ".oft", ".one", ".onetoc",
    ".ott", ".p7c", ".p7m", ".p7s", ".pem", ".potm", ".prc", ".prf", ".pub", ".qbb", ".qbw", ".qbx", ".qby",
    ".qdf", ".qdt", ".qed", ".qfx", ".qif", ".rft", ".shs", ".sig", ".sldm", ".sldx", ".sql", ".thmx",
    ".tlb", ".udl", ".udt", ".vcs", ".vtx", ".wbk", ".wiz", ".xla", ".xlam", ".xlk", ".xll", ".xltm", ".xltx",
    ".xnk", ".xps", ".xsd", ".xsl", ".xslt", ".xsn", ".xtp", ".xtps"
};



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

        //Перевірка чи має декілька розширень, +-працює
        private static bool MultipleExtensions(string file, out int count)
        {
            count = 0;
            string extension;

            do
            {
                extension = Path.GetExtension(file).ToLower();
                if (extension.Length != 0 && commonExtensions.Contains(extension))
                {
                    count++;
                    file = file.Remove(file.Length - extension.Length, extension.Length);
                }
                else
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(extension));

            return count > 1;
        }

        //Main logic
        private static bool addInitKostil = true;
        private static int scanedFiles = 0;
        private static int scanedDirs = 0;

        private static int foundFiles = 0;
        private static int foundDirs = 0;

        private static HashSet<string> processedDirs = new HashSet<string>();
        private static void ScanDirHelper(string directory)
        {
            List<string> dirs = GetDirs(directory);
            if (addInitKostil)
            {
                addInitKostil = false;
                dirs.Insert(0, directory);
            }

            scanedDirs += dirs.Count;

            foreach (string dir in dirs)
            {
                if (processedDirs.Contains(dir)) continue;
                processedDirs.Add(dir);

                List<string> files = GetFiles(dir);
                scanedFiles += files.Count;

                //Check if folder contains any mirrored files or has keyword files or >1 extension files
                bool hasKeyWordFiles =
                    files.Any(file => file.Contains(mirror_malware) ||
                                      malware_extensions.Any(me => file.EndsWith(me)) ||
                                      MultipleExtensions(file, out _)
                                      );

                //If directory contains suspicious files
                if (hasKeyWordFiles)
                {
                    foundDirs++;
                    Console.Write("\n" + dir + "\n");

                    foreach (string file in files)
                    {

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
                            Logger.Log(dir + @"\" + file, "extension");
                        }

                        //High chance for false-positive results
                        else if (MultipleExtensions(file, out int count))
                        {
                            foundFiles++;
                            Logger.CW("\t" + file, ConsoleColor.DarkYellow);
                            Logger.Log(dir + @"\" + file, "multiple extensions");

                        }

                    }
                }

                //If directory has directory inside, scan it
                if (GetDirs(dir).Count > 0)
                    ScanDirHelper(dir);
            }

        }

        public static void ScanDir(string directory)
        {
            if (!Directory.Exists(directory)) return;

            ScanDirHelper(directory);
            Console.Write(
    "\n\nScanned dirs: " + scanedDirs.ToString("N0") +
    "\nScanned files: " + scanedFiles.ToString("N0") +
    "\n\nDirs with malware files: " + foundDirs.ToString("N0") +
    "\nMalware files: " + foundFiles.ToString("N0"));



        }
    }
}
