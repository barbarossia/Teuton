using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Utility.Common;
using Utility.Progress;

namespace Mail
{
    public class GMail
    {
        private MailAddress fromAddress = new MailAddress("barbarossia.gol@gmail.com", "Barbarossia");
        private MailAddress toAddress = new MailAddress("barbarossia.gol@free.kindle.com", "Kindle");
        private MailAddress ccAddress = new MailAddress("barbarossia.gol@gmail.com", "Barbarossia");
        private IProgress<TaskAsyncProgress> progress;

        public GMail()
            : this(null)
        {
        }

        public GMail(IProgress<TaskAsyncProgress> progressReport)
        {
            this.progress = progressReport;
        }

        public void SendMail(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ApplicationException("Path is empty");

            if (!System.IO.File.Exists(path))
                throw new ApplicationException("File is not found");

            Attachment attachment = new Attachment(path);
            string subject = System.IO.Path.GetFileNameWithoutExtension(path);
            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Attachments.Add(attachment);
                message.CC.Add(ccAddress);
                using (var client = GetSmtpClient())
                {
                    this.progress.ReportStatus(this, subject, Status.Progressing);
                    client.Send(message);
                }
                this.progress.ReportStatus(this, subject, Status.Completed);
            }
        }

        public void SendMailAsync(string path, CancellationToken token)
        {
            if (string.IsNullOrEmpty(path))
                throw new ApplicationException("Path is empty");

            if (!System.IO.File.Exists(path))
                throw new ApplicationException("File is not found");

            Attachment attachment = new Attachment(path);
            string subject = System.IO.Path.GetFileNameWithoutExtension(path);
            var message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Attachments.Add(attachment);
            message.CC.Add(ccAddress);
            var client = GetSmtpClient();

            client.SendCompleted += (s, e) =>
            {
                if (e.Cancelled)
                {
                    this.progress.ReportStatus(this, subject, Status.Canceled);
                }
                else if (e.Error != null)
                {
                    this.progress.ReportStatus(this, subject, Status.Failed);
                }
                else
                {
                    this.progress.ReportStatus(this, subject, Status.Completed);
                }
                message.Dispose();
                client.Dispose();
            };

            client.SendAsync(message, null);
            if (token.IsCancellationRequested)
            {
                client.SendAsyncCancel();
            }
            this.progress.ReportStatus(this, subject, Status.Progressing);
        }

        private SmtpClient GetSmtpClient()
        {
            var smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("barbarossia.gol@gmail.com", "pbxlwhzonplrzwgh");
            return smtp;
        }
    }
}
