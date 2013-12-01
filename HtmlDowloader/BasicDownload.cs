using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Web;
using Utility.IO;

namespace HtmlDowload
{
    public class BasicDownload : IDownloader
    {
        public virtual string Download(DownloadContent file)
        {
            return DownloadFileToTemp(file.DownloadUrl, file.Title);
        }

        public virtual string[] Download(List<DownloadContent> file)
        {
            throw new ArgumentException();
        }

        protected string DownloadFileToTemp(string url, string fileName)
        {
            string tempFile = fileName.GetTempPath();
            WebUtility client = new WebUtility();
            client.DownloadFile(url, tempFile);
            return tempFile;
        }

        protected DownloadFile GetFileFromContent(DownloadContent content)
        {
            if (content == null)
                throw new ArgumentNullException("DownloadContent");
            if (content.DownloadUrl == null)
                throw new ArgumentNullException("DownloadUrl");

            var queryString = string.Join(string.Empty, content.DownloadUrl.Split('?').Skip(1));
            var queryStringCollection = queryString.ParseQueryString();
            return new DownloadFile()
            {
                Name = queryStringCollection["n"],
                Id = queryStringCollection["id"],
                UniqueHash = queryStringCollection["uhash"]
            };
        }
    }
}
