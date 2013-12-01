using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.ExceptionHandler
{
    [Serializable]
    public class ServiceInvalidTorrent : ServiceBaseException
    {
        public ServiceInvalidTorrent(long code)
            : base(code, string.Empty)
        {
        }

        public ServiceInvalidTorrent(long code, string message)
            : base(code, message, 0)
        {
        }

        public ServiceInvalidTorrent(long code, string message, long requestId) :
            base(code, message, requestId)
        {
        }
    }
}
