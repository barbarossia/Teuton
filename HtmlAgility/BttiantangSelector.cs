using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace HtmlAgility
{
    public class BttiantangSelector : ISearchResultSelector
    {
        public string Keyword { get; set; }
        public string LargeLimitedSize { get; set; }
        public string MinLimitedSize { get; set; }
        private FileSizeConverter converter = new FileSizeConverter();

        public BttiantangSelector()
        {
            Keyword = "720";
            LargeLimitedSize = "3.5GB";
            MinLimitedSize = "1GB";
        }

        public virtual DownloadContent Select(IEnumerable<DownloadContent> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (!list.Any())
                throw new ArgumentNullException("list");

            if (list.Count() == 1)
                return list.First();

            long largeLimitedSize = converter.Parse(LargeLimitedSize);
            long minLimitedSize = converter.Parse(MinLimitedSize);

            //bool hasKeyword = list.Any(l => l.Title.Contains(Keyword));
            bool hasNeededSize = list.Any(l => converter.Parse(l.Size) >= minLimitedSize && converter.Parse(l.Size) <= largeLimitedSize);
            bool hasOnlyOverLargeSize = list.All(l => converter.Parse(l.Size) > largeLimitedSize);         
            bool hasOnlyUnderMinSize = list.All(l => converter.Parse(l.Size) < minLimitedSize);

            IEnumerable<DownloadContent> query = list;

            //if (hasKeyword)
            //{
            //    query = query.Where(q => q.Title.Contains(Keyword));
            //}
            if (hasNeededSize)
            {
                query = GetNeededSize(query, minLimitedSize, largeLimitedSize);
            }
            else if (hasOnlyOverLargeSize)
            {
                query = query.OrderBy(q => converter.Parse(q.Size));
            }
            else if (hasOnlyUnderMinSize)
            {
                query = query.OrderByDescending(q => converter.Parse(q.Size));
            }

            return query.FirstOrDefault();
        }

        private IEnumerable<DownloadContent> GetNeededSize(IEnumerable<DownloadContent> list, long minLimitedSize, long largeLimitedSize)
        {
            return list.Where(q => converter.Parse(q.Size) >= minLimitedSize && converter.Parse(q.Size) <= largeLimitedSize)
                    .OrderByDescending(q => converter.Parse(q.Size));
        }
    }
}
