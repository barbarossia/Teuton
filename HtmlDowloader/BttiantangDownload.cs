using DataContract;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Web;
using Utility.IO;

namespace HtmlDowload
{
    public class BttiantangDownload : BasicDownload, IDownloader
    {
        private const string downloadUrl = "http://www.bttiantang.com/download.php";

        public override string Download(DownloadContent file)
        {
            var fileInfo = GetFileFromContent(file);

            var client = new RestClient(downloadUrl);
            var request = new RestRequest(Method.POST);
            request.AddParameter("action", "download");
            request.AddParameter("id", fileInfo.Id);
            request.AddParameter("uhash", fileInfo.UniqueHash);
            byte[] response = client.DownloadData(request);
            string tempFile = fileInfo.Name.GetTempPath();
            response.ToFile(tempFile);

            return tempFile;
        }

    }
}
