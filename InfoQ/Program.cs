using HtmlAgilityPack;
using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;
using Utility.Web;
using System.Text.RegularExpressions;
using System.IO;

namespace InfoQ
{
    class Program
    {
        private const string UserCookie = "Dp0LMRr2cHGrJ4sFSm6OSNu72CNOJmt3";
        private const string RegisteredUserCookie = "ii4EYQVPTBiX8Nnc9qcCMJZCxVbFa6HB";
        private static HtmlWeb webClient = new HtmlWeb();
        private static WebUtilityEx downloader;
        private static GMail mail = new GMail();
        private static string mainUrl = @"http://www.infoq.com";
        private static Regex re = new Regex(@".val\('(?<url>[\w\W]+?)'\)");

        static void Main(string[] args)
        {
            string url = @"http://www.infoq.com/cn/minibooks/architect-dec-10-2013";
            Dictionary<string, string> bookIndexes = GetIndex(url);
            foreach (var index in bookIndexes)
            {
                string downloadUrl = GetDownloadUrl(index.Value);
                Console.WriteLine("Get ebook download url: {0}", downloadUrl);
                string localPath = DownloadFile(index.Key, downloadUrl, index.Value);
                string fileName = localPath.GetFileName();
                Console.WriteLine("{0} downloaded.", fileName);
                SendMail(index.Key, localPath);
                Console.WriteLine("{0} sent.", fileName);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        static Dictionary<string, string> GetIndex(string url)
        {
            HtmlDocument htmlDoc = webClient.Load(url);
            HtmlNode div = htmlDoc.DocumentNode.SelectSingleNode(".//div[contains(@class,'about_authors')]");
            HtmlNodeCollection index = div.SelectNodes(".//a");

            return index.Select(link => new { link.InnerText, link.Attributes["href"].Value }).ToDictionary(t=>t.InnerText, t=>t.Value);
        }

        static string GetDownloadUrl(string url)
        {
            HtmlDocument htmlDoc = webClient.Load(url);
            HtmlNode div = htmlDoc.DocumentNode.SelectSingleNode(".//span[contains(@class,'downloadLinks')]");
            HtmlNodeCollection index = div.SelectNodes(".//a");
            var script = index.Select(link => link.Attributes["onclick"]).Last();

            return mainUrl + re.Match(script.Value).Groups["url"].Value;
        }

        static string DownloadFile(string title, string url, string referUrl)
        {
            string tempFile = title.GetValidFileName().GetTempPath();
            string ext = Path.GetExtension(new Uri(url).LocalPath);
            string fileName = tempFile + ext;
            if (downloader == null)
                downloader = new WebUtilityEx(UserCookie, RegisteredUserCookie, new Uri(mainUrl).Host);

            downloader.DownloadFile(url, fileName, referUrl);
           
            return fileName;
        }

        static void SendMail(string title, string path)
        {
            string subject = string.Format("Magazine: {0}", title);
            string message = string.Format("Attached is the {0} periodical downloaed by infoQ.", title);
            mail.SendMail(subject, message, path);
        }
    }
}
