using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract
{
    public class DownloadFile
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Id { get; set; }
        public string UniqueHash { get; set; }
    }
}
