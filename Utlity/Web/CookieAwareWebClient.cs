using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Web
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }
        public Uri Uri { get; set; }

        public CookieAwareWebClient()
            : this(new CookieContainer())
        {
        }

        public CookieAwareWebClient(CookieContainer cookies)
        {
            this.CookieContainer = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.CookieContainer;
            }
            HttpWebRequest httpRequest = (HttpWebRequest)request;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return httpRequest;
        }

        //protected override WebResponse GetWebResponse(WebRequest request)
        //{
        //    WebResponse response = base.GetWebResponse(request);
        //    String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

        //    if (setCookieHeader != null)
        //    {
        //        //do something if needed to parse out the cookie.
        //        if (setCookieHeader != null)
        //        {
        //            var dic = setCookieHeader.Split(';');
        //            var nameValue = dic[0].Split('=');
        //            Cookie cookie = new Cookie(nameValue[0], nameValue[1]) { Domain = response.ResponseUri.Host, Expires = DateTime.Now.AddDays(1.0) }; //create cookie
        //            this.CookieContainer.Add(cookie);
        //        }
        //    }
        //    return response;
        //}
    }
}
