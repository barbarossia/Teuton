using DataContract;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public class BttiantangRepository : SearchBaseRepository, ISearch
    {
        protected override List<DownloadContent> SearchCore(string url)
        {
            var webClient = new HtmlWeb();
            HtmlDocument htmlDoc = webClient.Load(url);
            string pattern = ".//div[contains(@class, 'tinfo')]";
            HtmlNodeCollection leftLinkNodes = htmlDoc.DocumentNode.SelectNodes(pattern);
            return leftLinkNodes.Select(l =>
                {
                    var a = l.SelectSingleNode(".//a");
                    return new DownloadContent()
                    {
                        DownloadUrl = a.Attributes["href"].Value,
                        Title = a.Attributes["title"].Value,
                        Size = a.SelectSingleNode(".//p/em").InnerText,
                        Files = ToFiles(l)
                    };
                }).ToList();
        }

        private List<DownloadFile> ToFiles(HtmlNode node)
        {
            string pattern = ".//ul/li/span[contains(@class, 'video') or contains(@class, 'file')]";
            var results = node.SelectNodes(pattern);
            if (results.Any())
            {
                return results.Select(f => 
                    {
                        DownloadFile file= new DownloadFile();
                        if (f.ChildNodes.Count == 1)
                        {
                            file.Name = f.ChildNodes[0].InnerText;
                        }
                        else
                        {
                            file.Name = f.ChildNodes[0].InnerText;
                            file.Size = f.ChildNodes[1].InnerText;
                        }
                        return file;
                    }).ToList();
            }

            return null;
        }
    }
}
