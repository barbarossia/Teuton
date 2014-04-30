using DataContract;
using Magnet2Bt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.IO;

namespace HtmlDowload
{
    public class YytesDownload : BasicDownload, IDownloader
    {
        private const string pattern = "magnet:?xt=urn:btih:";
        private const string downloadUrl = "http://d1.torrentkittycn.com/?infohash={0}";

        public override string Download(DownloadContent file)
        {
            string magnetUrl = file.DownloadUrl;
            int startIndex = pattern.Length;
            int endIndex = magnetUrl.IndexOf('&');
            string sha1 = magnetUrl.Substring(startIndex, endIndex - startIndex);

            string url = string.Format(downloadUrl, sha1);
            string fileName = file.Title.FileChangeExtension("torrent");
            return DownloadFileToTemp(url, fileName);
        }
    }
}
