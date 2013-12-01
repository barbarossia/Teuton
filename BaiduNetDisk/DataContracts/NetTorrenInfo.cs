using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.DataContracts
{
    public class NetTorrenInfo
    {
        public string SHA1 { get; set; }
        public int FileCount { get; set; }
        public List<NetFileInfo> Files { get; set; }
    }
}
