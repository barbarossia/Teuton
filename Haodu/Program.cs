using DataContract;
using KindelConverter;
using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Utility.Logging;

namespace Haodu
{
    class Program
    {
        static ILogger Logger { get; set; }
        static IMail Mail { get; set; }
        static IMail MailToKindle { get; set; }
        static EpubConverter converter;

        static void Main(string[] args)
        {
        }

        static void InitComponent()
        {
            MailAddress fromAddress = new MailAddress("barbarossia.gol@gmail.com", "Barbarossia");
            MailAddress ToAddress = new MailAddress("barbarossia.gol@free.kindle.com", "Kindle");

            Mail = new GMail(fromAddress);
            MailToKindle = new GMail(ToAddress);
        }

        static void SendEmailToSelf(DownloadAction action)
        {
            SendMail(action, Mail);
        }

        static void SendEmailToKindle(DownloadAction action)
        {
            SendMail(action, MailToKindle);
        }

        static bool Convert(DownloadAction action)
        {
            return converter.Convert(action.TempFile);
        }

        static void SendMail(DownloadAction action, IMail client)
        {
            try
            {
                string title = action.Content.Title;
                string subject = string.Format("Book: {0}", title);
                string message = string.Format(@"Attached is the {0} downloaed by http://www.haodoo.net/.", title);
                client.SendMail(subject, message, action.TempFile);
            }
            catch (Exception ex)
            {
                Logger.Log(MessageLevel.Error, ex.Message, ex);
            }
        }
    }
}
