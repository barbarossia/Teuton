using DataContract;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public abstract class SearchBaseRepository : ISearch
    {
        public ISearchResultSelector Selector { get; set; }

        public DownloadContent Search(string url)
        {
            var result = SearchCore(url);
            return Selector.Select(result);
        }

        protected abstract List<DownloadContent> SearchCore(string url);

    }
}
