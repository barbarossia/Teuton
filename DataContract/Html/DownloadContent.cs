using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract
{
    public class DownloadContent
    {
        public string DownloadUrl { get; set; }
        public string Title { get; set; }
        public string Size { get; set; }
        public List<DownloadFile> Files { get; set; }

    }
}
