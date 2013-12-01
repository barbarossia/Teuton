using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.DataContracts
{
    public class NetFileInfo
    {
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string MD5 { get; set; }
        public long Size { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
