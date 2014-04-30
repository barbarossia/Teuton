using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace Utility.Web
{
    /// <summary>
    /// Get URL for IE Favorites folder
    /// Refer: http://social.msdn.microsoft.com/Forums/vstudio/en-US/23dcaca6-52f8-40f4-a2f8-4f9029e15131/ie-favorites-how-to-read-title-url-from-url-file-in-c?forum=csharpgeneral
    /// URL Refer: http://www.fmtz.com/formats/url-file-format/article#urlformat
    /// </summary>
    public static class Bookmark
    {
        public static string[] GetIEFavoritesUrl()
        {
            return GetIEFavoritesUrl(string.Empty);
        }

        public static string[] GetIEFavoritesUrl(string folder)
        {
            string ieFavoriatePath = GetFavoritePath(folder);

            if (!ieFavoriatePath.DirectoryExists())
                throw new ArgumentOutOfRangeException(folder);

            return ieFavoriatePath.GetFiles();
        }

        public static string GetUrl(string file)
        {
            string url = string.Empty;
            using (StreamReader rdr = new StreamReader(file))
            {
                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    if (line.StartsWith("URL=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (line.Length > 4)
                            url = line.Substring(4);
                        else
                            url = "";
                        break;
                    }
                }
                // ... move on to the next file   
            }

            return url;
        }

        public static void DeleteBookmark(string folder, string name)
        {
            string ieFavoriatePath = GetFavoritePath(folder);

            if (!ieFavoriatePath.DirectoryExists())
                throw new ArgumentOutOfRangeException(folder);

            string file = ieFavoriatePath.GetFullPath(name);

            if (!file.FileExists())
                throw new ArgumentOutOfRangeException(file);

            file.DeleteFile();
        }

        private static string GetFavoritePath(string folder)
        {
            string ieFavoriatePath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            if (!string.IsNullOrEmpty(folder))
                ieFavoriatePath = ieFavoriatePath.GetFullPath(folder);

            return ieFavoriatePath;
        }
    }
}
