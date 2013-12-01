using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.IO;
using Utility.Progress;

namespace Utility.Web
{
    public class WebUtility
    {
        private IProgress<TaskAsyncProgress> progress;

         public WebUtility()
            : this(null)
        {
        }

         public WebUtility(IProgress<TaskAsyncProgress> progressReport)
        {
            this.progress = progressReport;
        }

        public void DownloadFileAsync(string url, string dir)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException("dir");

            if (!IOExtenstions.DirectoryExists(dir))
                throw new ArgumentOutOfRangeException("dir not exist");

            // Create an instance of WebClient
            WebClient client = new WebClient();

            Uri uri = new Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            // Hookup DownloadFileCompleted Event
            client.DownloadFileCompleted += (s, e) =>
            {
                if (e.Cancelled)
                {
                    this.progress.ReportStatus(this, fileNameWithoutExtension, Status.Canceled);
                }
                else if (e.Error != null)
                {
                    this.progress.ReportStatus(this, fileNameWithoutExtension, Status.Failed);
                }
                else
                {
                    this.progress.ReportStatus(this, fileNameWithoutExtension, Status.Completed);
                }
                client.Dispose();
            };

            // Start the download and copy the file to c:\temp
            client.DownloadFileAsync(new Uri(url), IOExtenstions.GetFullPath(dir, fileName));

        }

        public void DownloadFile(string url, string file)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            // Create an instance of WebClient
            WebClient client = new WebClient();

            // Start the download and copy the file to c:\temp
            client.DownloadFile(new Uri(url), file);
        }
    }
}
