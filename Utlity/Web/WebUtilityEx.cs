using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Utility.Web
{
    public class WebUtilityEx
    {
        private CookieAwareWebClient http;
        public string UserCookie { get; private set; }
        public string RegisteredUserCookie { get; private set; }
        public string Host {get; private set;}

        public WebUtilityEx(string userCookie, string registeredUserCookie, string host)
        {
            this.UserCookie = userCookie;
            this.RegisteredUserCookie = registeredUserCookie;
            this.Host = host;

            CookieContainer cookieJar = new CookieContainer();
            cookieJar.Add(new Cookie(ReflectionUtility.GetPropertyName(() => this.UserCookie), UserCookie) { Domain = host });
            cookieJar.Add(new Cookie(ReflectionUtility.GetPropertyName(() => this.RegisteredUserCookie),RegisteredUserCookie) { Domain = host });
            http = new CookieAwareWebClient(cookieJar);
        }

        public void DownloadFile(string url, string file, string referUrl)
        {
            http.DownloadFile(new Uri(url), file);
        }
    }
}
