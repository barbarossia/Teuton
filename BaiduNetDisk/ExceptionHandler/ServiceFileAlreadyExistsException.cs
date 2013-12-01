using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.ExceptionHandler
{
    [Serializable]
    public class ServiceFileAlreadyExistsException : ServiceBaseException
    {
        public ServiceFileAlreadyExistsException(long code)
            : base(code, string.Empty)
        {
        }

        public ServiceFileAlreadyExistsException(long code, string message)
            : base(code, message, 0)
        {
        }

        public ServiceFileAlreadyExistsException(long code, string message, long requestId) :
            base(code, message, requestId)
        {
        }

    }
}
