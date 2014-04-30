using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgility
{
    public class YytesSelector : BttiantangSelector, ISearchResultSelector
    {
        public override DownloadContent Select(IEnumerable<DownloadContent> list)
        {
            var filter = list.Where(l => l.DownloadUrl.Contains("magnet:"));
            return base.Select(filter);
        }
    }
}
