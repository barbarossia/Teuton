using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.ExceptionHandler
{
    [Serializable]
    public class ServiceBaseException : Exception
    {
        public long ErrorCode { get; set; }
        public string Message { get; set; }
        public long RequestId { get; set; }

        public ServiceBaseException(long code)
            : this(code, string.Empty)
        {
        }

        public ServiceBaseException(long code, string message)
            : this(code, message, 0)
        {
        }

        public ServiceBaseException(long code, string message, long requestId)
        {
            ErrorCode = code;
            Message = message;
            RequestId = requestId;
        }
    }
}
