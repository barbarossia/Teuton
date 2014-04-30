using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessToken
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(
                OnUnhandledException);

            string apiKey = string.Empty;
            string authorizationCode = string.Empty;
            string securityCode = string.Empty;
            string accessToken = string.Empty;

            Console.WriteLine("Enter your API key:");
            apiKey = Console.ReadLine();
            Service.GetAuthorizationCode(apiKey);

            Console.WriteLine("Enter your authorization code:");
            authorizationCode = Console.ReadLine();

            Console.WriteLine("Enter your security code:");
            securityCode = Console.ReadLine();
            accessToken = Service.GetAccessToken(apiKey, authorizationCode, securityCode);

            Console.WriteLine("Access Token is {0}", accessToken);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //here's how you get the exception   
            Exception exception = (Exception)e.ExceptionObject;


            //bail out in a tidy way and perform your logging 
            Console.Error.WriteLine(exception.ToString());
            Console.Error.WriteLine(exception.StackTrace);
            if (exception.InnerException != null)
            {
                Console.Error.WriteLine(exception.InnerException.ToString());
                Console.Error.WriteLine(exception.InnerException.StackTrace);
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            //Environment.Exit(666); 
        }

    }
}
