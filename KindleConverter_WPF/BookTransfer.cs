using KindelConverter;
using KindleConverter_WPF.Models;
using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility.Common;
using Utility.IO;
using Utility.Logging;
using Utility.Progress;

namespace KindleConverter_WPF
{
    public class BookTransfer
    {
        private ILogger logger;
        private string INPUT = ConfigManager.AppSettings["Input"];
        private string OUTPUT = ConfigManager.AppSettings["Output"];
        private string HISTORY = ConfigManager.AppSettings["History"];
        private string FAIL = ConfigManager.AppSettings["Fail"];
        private const string MOBIExtension = "mobi";
        private const string EPUBExtension = "epub";
        private Progress<TaskAsyncProgress> progress;
        private GMail mail;
        private EpubConverter convert;
        private CancellationTokenSource token;

        public CancellationTokenSource Token
        {
            get { return token; }
        }

        public BookTransfer(ILogger logger, 
            Action<TaskAsyncProgress> progressChanged,
            CancellationTokenSource token)
        {
            this.logger = logger != null ? logger : NullLogger.Instance;
            progress = new Progress<TaskAsyncProgress>(progressChanged);
            mail = new GMail(null, null, progress);
            convert = new EpubConverter(progress);
            this.token = token;
        }

        public void BeforeConvert(Book book)
        {
            OnException(() =>
                {
                    string fileName = book.FilePath.GetFileName();
                    string tempPath = fileName.GetTempPath();
                    if (tempPath.FileExists())
                    {
                        tempPath.DeleteFile();
                    }

                    book.FilePath.FileCopy(tempPath);
                    book.TempFilePath = tempPath;

                    string mobiPath = book.TempFilePath.FileChangeExtension(MOBIExtension);
                    if (mobiPath.FileExists())
                    {
                        mobiPath.DeleteFile();
                    }

                }, "Before Convert", book);

        }

        public void AfterConvert(Book book)
        {
            OnException(() =>
            {
                if (book.Status == Status.Completed)
                {
                    string mobiPath = book.TempFilePath.FileChangeExtension(MOBIExtension);
                    book.MobiFilePath = mobiPath;
                }
                else if (book.Status == Status.Failed)
                {
                    book.FilePath.FileCopyToDirctory(FAIL);
                }
            }, "After Convert", book);
        }

        public void Convert(Book book)
        {
            OnException(() =>
            {
               convert.Convert(book.TempFilePath);
            }, "Convert", book);
        }

        public void Send(Book book)
        {
            OnException(() =>
            {
                mail.SendMail(book.MobiFilePath);
            }, "Send", book);
        }

        public void AfterSend(Book book)
        {
            OnException(() =>
            {
                if (book.Status == Status.Completed)
                {
                    book.FilePath.FileCopyToDirctory(HISTORY);
                    book.FilePath.DeleteFile();
                }
                else if (book.Status == Status.Failed)
                {
                    book.FilePath.FileCopyToDirctory(FAIL);
                    book.FilePath.DeleteFile();
                }
            }, "After Send", book);
        }

        private void OnException(Action action, string info, Book book)
        {
            try
            {
                logger.Log(MessageLevel.Info, info);
                action();
            }
            catch (Exception ex)
            {
                book.Status = Status.Error;
                logger.Log(MessageLevel.Error, ex.Message, ex);
            }
        }
    }
}
