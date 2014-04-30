using RestSharp;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToken
{
    public class Service
    {
        private const string authorizeUrl = "https://openapi.baidu.com/oauth/2.0/authorize?response_type=code&client_id={0}&redirect_uri=oob&scope=netdisk";
        private const string accessTokenUrl = "https://openapi.baidu.com/oauth/2.0/token?grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}&redirect_uri=oob";

        public static void GetAuthorizationCode(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey");

            string url = string.Format(authorizeUrl, apiKey);

            Launcher.Launch(url);
        }

        public static string GetAccessToken(string apiKey, string authorizationCode, string securityCode)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrWhiteSpace(authorizationCode))
                throw new ArgumentNullException("authorizationCode");
            if (string.IsNullOrWhiteSpace(securityCode))
                throw new ArgumentNullException("securityCode");


            string url = string.Format(accessTokenUrl, authorizationCode, apiKey, securityCode);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            JsonValue content = System.Json.JsonObject.Parse(response.Content);

            return (string)content["access_token"];
        }
    }
}
