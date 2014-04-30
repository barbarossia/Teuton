using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.ExceptionHandler
{
    [Serializable]
    public class ServiceAuthoriazationException : ServiceBaseException
    {
        public ServiceAuthoriazationException(long code)
            : base(code, string.Empty)
        {
        }

        public ServiceAuthoriazationException(long code, string message)
            : base(code, message, 0)
        {
        }

        public ServiceAuthoriazationException(long code, string message, long requestId) :
            base(code, message, requestId)
        {
        }
    }
}
