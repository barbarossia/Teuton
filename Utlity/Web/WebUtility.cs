using System;
using System.IO;
using System.Net;
using Utility.Common;
using Utility.IO;
using Utility.Progress;
using System.Linq;

namespace Utility.Web
{
    public class WebUtility
    {
        private IProgress<TaskAsyncProgress> progress;
        private WebClient client;
        private string login;
        private string password;

        public WebUtility(string login, string password)
            : this(login, password, null)
        {

        }

         public WebUtility()
            : this(string.Empty, string.Empty, null)
        {
        }

         public WebUtility(string login, string password, IProgress<TaskAsyncProgress> progressReport)
        {
            client = new WebClient();
            //client.Proxy = new WebProxy("127.0.0.1");
            this.login = login;
            this.password = password;

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

        private void FillCredentials(string url)
        {
            NetworkCredential myCred = new NetworkCredential(login, password);

            string[] hostParts = new Uri(url).Host.Split('.');
            string domain = String.Join(".", hostParts.Skip(Math.Max(0, hostParts.Length - 2)).Take(2));
            myCred.Domain = domain;

            client.Credentials = myCred;
        }

        public void DownloadFile(string url, string file)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            // Start the download and copy the file to c:\temp
            //FillCredentials(url);
            client.DownloadFile(new Uri(url), file);
        }

        public void DownloadFile(string url, string file, string referUrl)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            // Start the download and copy the file to c:\temp
            client.Headers.Add("Referer", referUrl);
            client.DownloadFile(new Uri(url), file);
        }
    }
}
