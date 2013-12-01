using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.ExceptionHandler
{
    [Serializable]
    public class WebProtocolException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public WebProtocolException(HttpStatusCode statusCode)
            : this(statusCode, GetDefaultStatusDescription(statusCode), null)
        {
        }

        public WebProtocolException(HttpStatusCode statusCode, string message)
            : this(statusCode, message, null)
        {
        }

        public WebProtocolException(HttpStatusCode statusCode, string statusDescription, Exception innerException)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        static string GetDefaultStatusDescription(HttpStatusCode statusCode)
        {
            return statusCode.ToString();
        }

    }
}
