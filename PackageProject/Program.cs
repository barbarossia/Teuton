using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utlity.IO;

namespace PackageProject
{
    class Program
    {
        static void Main(string[] argv)
        {
            if (argv.Length < 1)
            {
                Console.WriteLine("Usage: CmprDir.exe <in_dir compressed_file>");
                return;
            }

            try
            {
                string directory = argv[0];
                string file = argv[1];
                if (IOExtenstions.DirectoryExists(directory) || 
                    (!string.IsNullOrEmpty(file) && !IOExtenstions.FileExists(file)))
                {
                    CompressProject(directory, file);
                }
                else
                {
                    Console.Error.WriteLine("Wrong arguments");
                }

                return;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return;
            }
        }

        static void CompressProject(string projectPath, string fileName)
        {
            CleanUpProjectFolder(projectPath);
            if (string.IsNullOrEmpty(fileName))
                Compressor.CompressDirectory(projectPath);
            else
                Compressor.CompressDirectory(projectPath, fileName);
        }

        private static void CleanUpProjectFolder(string projectPath)
        {
            foreach (var path in IOExtenstions.Search(projectPath, "bin|obj"))
            {
                IOExtenstions.DeleteDirectorySafe(path, true);
            }
        }
    }
}
