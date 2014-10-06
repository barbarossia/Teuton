using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility.Web
{
    public static class HttpExtentions
    {
        private static Encoding encoding = Encoding.UTF8;

        public static string UrlDecode(this string url)
        {
            return HttpUtility.UrlDecode(url, encoding);
        }

        public static string UrlEncode(this string url)
        {
            return HttpUtility.UrlEncode(url, encoding);
        }

        public static NameValueCollection ParseQueryString(this string queryString)
        {
            return HttpUtility.ParseQueryString(queryString);
        }
    }
}
