using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.IO
{
    public static class IOExtenstions
    {
        public static void ToFile(this byte[] byteArray, string fileName)
        {
            // Open file for reading
            System.IO.FileStream _FileStream =
               new System.IO.FileStream(fileName, System.IO.FileMode.Create,
                                        System.IO.FileAccess.Write);
            // Writes a block of bytes to this stream using data from
            // a byte array.
            _FileStream.Write(byteArray, 0, byteArray.Length);

            // close file stream
            _FileStream.Close();
        }

        public static long GetFileSize(this string fileName)
        {
            FileInfo f = new FileInfo(fileName);
            return f.Length;
        }

        public static string GetValidFileName(this string fileName)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(fileName, "");
        }

        public static string[] ShowOpenFileDialogAndReturnResult(string filter, string dialogTitle)
        {
            string[] fileNames = new string[] { };
            var openFileDialog = CreateOpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = true;

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = "Open";
            }

            openFileDialog.Title = dialogTitle;
            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileNames = openFileDialog.FileNames;
            }

            return fileNames;
        }

        public static string[] GetFiles(this string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir not be empty.");

            if (!Directory.Exists(dir))
                throw new ArgumentNullException("dir is not exist.");

            return Directory.GetFiles(dir);
        }

        public static IEnumerable<string> Search(string path, string filter)
        {
            string dirName = Path.GetFileNameWithoutExtension(path);
            string[] pattern = filter.Split('|');
            var query = pattern.Where(p => p.Equals(dirName, StringComparison.OrdinalIgnoreCase));
            if (query.Any())
                yield return path;

            if (Directory.Exists(path))
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    foreach (var subDir in Search(dir, filter))
                    {
                        yield return subDir;
                    }
                }
            }
        }

        public static void DeleteFile(this string file)
        {
            File.Delete(file);
        }

        public static void DeleteDirectorySafe(string path, bool recursive)
        {
            DoSafeAction(() => DeleteDirectory(path, recursive));
        }

        private static void DeleteDirectory(string path, bool recursive)
        {
            if (!DirectoryExists(path))
            {
                return;
            }

            try
            {
                Directory.Delete(path, recursive);

                // The directory is not guaranteed to be gone since there could be
                // other open handles. Wait, up to half a second, until the directory is gone.
                for (int i = 0; Directory.Exists(path) && i < 5; ++i)
                {
                    System.Threading.Thread.Sleep(100);
                }

            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        public static bool DirectoryExists(this string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            return Directory.Exists(directory);
        }

        public static bool FileExists(this string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            return File.Exists(file);
        }

        public static string GetFullPath(this string root, string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return root;
            }
            return Path.Combine(root, path);
        }

        public static string GetTempPath(this string path)
        {
            return GetFullPath(GetTempDirectoryPath(), path);
        }

        public static string GetFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(this string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string GetTempDirectoryPath()
        {
            return Path.GetTempPath();
        }

        public static void FileMove(this string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public static void FileMoveToDirctory(string sourceFileName, string destDirctory)
        {
            string fileName = IOExtenstions.GetFileName(sourceFileName);
            string destFileName = IOExtenstions.GetFullPath(destDirctory, fileName);
            FileMove(sourceFileName, destFileName);
        }

        public static void FileCopy(this string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        public static void FileCopyToDirctory(this string sourceFileName, string destDirctory)
        {
            string fileName = IOExtenstions.GetFileName(sourceFileName);
            string destFileName = IOExtenstions.GetFullPath(destDirctory, fileName);
            FileCopy(sourceFileName, destFileName);
        }

        public static string FileChangeExtension(this string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        public static FileStream ToStream(this string path)
        {
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                //fs.Close();

                return fs;
            }
        }

        public static string ToText(this string path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path))
            {
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();

        }



        public static void ToFile(this string content, string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
            {
                file.WriteLine(content);
            }
        }

        public static Func<OpenFileDialog> CreateOpenFileDialog = () => new OpenFileDialog();

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to log an exception as a warning and move on")]
        private static void DoSafeAction(Action action)
        {
            try
            {
                Attempt(action);
            }
            catch (Exception)
            {
            }
        }

        private static void Attempt(Action action, int retries = 3, int delayBeforeRetry = 150)
        {
            while (retries > 0)
            {
                try
                {
                    action();
                    break;
                }
                catch
                {
                    retries--;
                    if (retries == 0)
                    {
                        throw;
                    }
                }
                Thread.Sleep(delayBeforeRetry);
            }
        }
    }
}
