using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.DataContracts
{
    public class ServiceError
    {
        public long ErrorCode { get; set; }
        public string Message { get; set; }
        public long RequestId { get; set; }
    }
}
