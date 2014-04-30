using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public interface ISearch
    {
        ISearchResultSelector Selector { get; set; }
        DownloadContent Search(string url);
    }
}
