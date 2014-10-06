using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.IO
{
    public static class Compressor
    {
        public static void CompressDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            if (!Directory.Exists(directory))
                throw new ArgumentException("directory not exists");

            DirectoryInfo parentDirectoryPath = Directory.GetParent(directory);
            string fileName = Path.Combine(parentDirectoryPath.FullName, Path.GetFileNameWithoutExtension(directory) + ".zip");
        }

        public static void CompressDirectory(string directory, string fileName)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!Directory.Exists(directory))
                throw new ArgumentException("directory not exists");
            if (File.Exists(fileName))
                throw new ArgumentException("file has been exists");

            ZipFile.CreateFromDirectory(directory, fileName);
        }
    }
}
