using DataContract;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public class YyetsRepository : SearchBaseRepository, ISearch
    {
        protected override List<DownloadContent> SearchCore(string url)
        {
            var webClient = new HtmlWeb();
            HtmlDocument htmlDoc = webClient.Load(url);

            HtmlNode tab = htmlDoc.DocumentNode.SelectSingleNode(".//div[@id='tabs']");
            HtmlNode div = tab.SelectSingleNode(".//div[contains(@class,'box_1')]");
            HtmlNode ul = div.SelectSingleNode(".//ul");
            HtmlNodeCollection li = ul.SelectNodes(".//li");

            List<DownloadContent> result = new List<DownloadContent>();

            foreach (var l in li)
            {
                var titleDiv = l.SelectSingleNode(".//div[contains(@class,'lks')]");
                var title = titleDiv.SelectSingleNode(".//span[contains(@class,'a')]");
                var size = titleDiv.SelectSingleNode(".//font[contains(@class,'f5')]");

                var downloadDiv = l.SelectSingleNode(".//div[contains(@class,'pks')]");
                var links = downloadDiv.SelectNodes(".//a");

                foreach (var link in links)
                {
                    if (link.HasAttributes && link.Attributes.Contains("href"))
                    {
                        result.Add(new DownloadContent()
                        {
                            DownloadUrl = link.Attributes["href"].Value,
                            Title = title.InnerText,
                            Size = size != null ? size.InnerText : "0",
                        });
                    }
                }
            }

            return result;
        }

    }
}
