using DataContract;
using Magnet2Bt;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;
using Utility.Web;

namespace HtmlDowload
{
    public class BttiantangBackupDownload : BasicDownload, IDownloader
    {
        private const string downloadUrl = "http://www.bttiantang.com/jumpto.php";

        public override string Download(DownloadContent file)
        {
            var fileInfo = GetFileFromContent(file);

            var client = new RestClient(downloadUrl);
            var request = new RestRequest(Method.GET);
            request.AddParameter("cqr", "yes");
            request.AddParameter("aid", fileInfo.Id);
            request.AddParameter("uhash", fileInfo.UniqueHash);

            var response = client.Execute(request);
            var redirectUrl = response.ResponseUri.AbsolutePath.UrlDecode();

            string sha1 = redirectUrl.Split(':').Last();
            string url = MBConverter.Convert(sha1);

            return DownloadFileToTemp(url, fileInfo.Name);
        }

    }
}
