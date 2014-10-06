using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Utility.Common;
using Utility.Progress;

namespace Mail
{
    public class GMail:IMail
    {
        private MailAddress fromAddress = new MailAddress("barbarossia.gol@gmail.com", "Barbarossia");
        private MailAddress DefaultToAddress = new MailAddress("barbarossia.gol@free.kindle.com", "Kindle");
        private MailAddress DefaultCcAddress = new MailAddress("barbarossia.gol@gmail.com", "Barbarossia");
        private MailAddress toAddress;
        private MailAddress ccAddress;
        private IProgress<TaskAsyncProgress> progress;

        public GMail()
            : this(null, null, null)
        {
        }

        public GMail(MailAddress to)
            :this(to, null, null)
        {
        }

        public GMail(MailAddress to, MailAddress cc)
            :this(to, cc, null)
        {
        }

        public GMail(MailAddress to, MailAddress cc, IProgress<TaskAsyncProgress> progressReport)
        {
            this.toAddress = to ?? DefaultToAddress;
            this.ccAddress = cc ?? DefaultCcAddress;
            this.progress = progressReport;
        }

        public void SendMail(string subject, string message, string attachmentPath)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ApplicationException("subject");
            if (string.IsNullOrEmpty(message))
                throw new ApplicationException("message");
            if (string.IsNullOrEmpty(attachmentPath))
                throw new ApplicationException("attachmentPath");

            if (!System.IO.File.Exists(attachmentPath))
                throw new ApplicationException("File is not found");

            Attachment attachment = new Attachment(attachmentPath);

            using (var mail = new MailMessage(fromAddress, toAddress))
            {
                mail.Subject = subject;
                mail.Body = message;
                mail.Attachments.Add(attachment);
                mail.CC.Add(ccAddress);
                using (var client = GetSmtpClient())
                {
                    this.progress.ReportStatus(this, subject, Status.Progressing);
                    client.Send(mail);
                }
                this.progress.ReportStatus(this, subject, Status.Completed);
            }
        }

        public void SendMail(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ApplicationException("Path is empty");

            if (!System.IO.File.Exists(path))
                throw new ApplicationException("File is not found");

            string subject = System.IO.Path.GetFileNameWithoutExtension(path);
            string message = subject;
            SendMail(subject, message, path);
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
