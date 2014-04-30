using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract
{
    public class DownloadAction
    {
        public string Source { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public DownloadContent Content { get; set; }
        public string RemotePath { get; set; }
        public string TempFile { get; set; }
        public Exception Exception { get; set; }
        public bool HasException
        {
            get
            {
                return Exception != null;
            }
        }
        public bool CanDelete { get; set; }
        public string TorrentPath { get; set; }
        public string SavePath { get; set; }
    }
}
