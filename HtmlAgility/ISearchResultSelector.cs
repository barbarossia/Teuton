using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public interface ISearchResultSelector
    {
        DownloadContent Select(IEnumerable<DownloadContent> list);
    }
}
