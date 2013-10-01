using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utlity.IO
{
    public static class IOExtenstions
    {
        public static long GetFileSize(this string fileName)
        {
            FileInfo f = new FileInfo(fileName);
            return f.Length;
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

        public static string[] GetFiles(string dir)
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

        public static bool DirectoryExists(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            return Directory.Exists(directory);
        }

        public static bool FileExists(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            return File.Exists(file);
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
