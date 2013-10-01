using KindelConverter;
using KindleConverter_WPF.Models;
using Mail;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utlity.Common;
using Utlity.IO;
using Utlity.Progress;

namespace KindleConverter_WPF.ViewModles
{
    public class MainWindowViewModel : NotificationObject
    {
        public DelegateCommand BrowseCommand { get; set; }
        public DelegateCommand ConvertCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public Window View { get; set; }
        private List<Book> books;
        private Book selectedItem;
        private BlockingCollection<Book> results;
        private CancellationTokenSource cancellationToken;
        private string INPUT = ConfigurationManager.AppSettings["Input"];
        private string OUTPUT = ConfigurationManager.AppSettings["Output"];
        private string HISTORY = ConfigurationManager.AppSettings["History"];
        private string FAIL = ConfigurationManager.AppSettings["Fail"];

        public List<Book> Books
        {
            get
            {
                return books;
            }
            set
            {
                books = value;
                RaisePropertyChanged(() => Books);
            }
        }

        public Book SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        private string status;
        public string AllBookStatus
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                RaisePropertyChanged(() => AllBookStatus);
            }
        }

        private bool enableCancel;
        public bool EnableCancel
        {
            get
            {
                return enableCancel;
            }
            set
            {
                enableCancel = value;
                RaisePropertyChanged(() => EnableCancel);
            }
        }

        public MainWindowViewModel()
        {
            BrowseCommand = new DelegateCommand(BrowseFile);
            ConvertCommand = new DelegateCommand(ConvertFiles);
            CancelCommand = new DelegateCommand(Cancel);
            EnableCancel = false;
        }

        private async void ConvertFiles()
        {
            cancellationToken = new CancellationTokenSource();
            EnableCancel = true;
            try
            {
                await ConvertBegin(Books).ContinueWith(resultTask =>
                {
                    AllBookStatus = "All Done";
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void Cancel()
        {
            cancellationToken.Cancel();
            EnableCancel = false;
            AllBookStatus = "Cancel";
        }

        private void OnCompleted()
        {
            AllBookStatus = "All Done";
        }

        private async Task ConvertBegin(List<Book> books)
        {
            results = new BlockingCollection<Book>(10);
            var progress = new Progress<TaskAsyncProgress>();
            GMail mail = new GMail(progress);
            EpubConverter convert = new EpubConverter(progress);
            progress.ProgressChanged += progress_ProgressChanged;

            List<Task> tasks = new List<Task>();
            var consume = Task.Run(() => Send(mail, results));

            foreach (var book in books)
            {
                await convert.ConvertAsync(book.FilePath, cancellationToken.Token);
                results.Add(book);
            }
        }

        private void Send(GMail mail, BlockingCollection<Book> results)
        {
            foreach (var item in results.GetConsumingEnumerable())
            {
                var currentItem = item;
                if (currentItem.Status == Status.Converted)
                {
                    string targetPath = Path.Combine(OUTPUT, currentItem.Name + ".mobi");
                    mail.SendMailAsync(targetPath, cancellationToken.Token);
                }
            }
        }

        private void progress_ProgressChanged(object sender, TaskAsyncProgress e)
        {
            FileOperations(e.Key, (Status)e.Statue);

            Book book = this.Books.FirstOrDefault(b => b.Name == e.Key);
            if (book != null)
            {
                book.Status = (Status)e.Statue;
            }           
        }

        private void FileOperations(string name, Status status)
        {
            string sourcePath;
            string targetPath;

            switch (status)
            {
                case Status.Converted:
                    sourcePath = Path.Combine(INPUT, name + ".mobi");
                    targetPath = Path.Combine(OUTPUT, name + ".mobi");
                    File.Move(sourcePath, targetPath);
                    break;
                case Status.ConvertedFail:
                    sourcePath = Path.Combine(INPUT, name + ".epub");
                    targetPath = Path.Combine(FAIL, name + ".epub");
                    File.Move(sourcePath, targetPath);
                    break;
                case Status.Done:
                    sourcePath = Path.Combine(INPUT, name + ".epub");
                    targetPath = Path.Combine(HISTORY, name + ".epub");
                    File.Move(sourcePath, targetPath);
                    break;
                case Status.SendFailed:
                    sourcePath = Path.Combine(INPUT, name + ".epub");
                    targetPath = Path.Combine(FAIL, name + ".epub");
                    File.Move(sourcePath, targetPath);
                    targetPath = Path.Combine(OUTPUT, name + ".mobi");
                    File.Delete(targetPath);
                    break;
            }
        }

        private void BrowseFile()
        {
            var fileNames = IOExtenstions.ShowOpenFileDialogAndReturnResult("Epub files (*.epub)|*.epub", "Open Epub File");

            Books = new List<Book>(fileNames.Select(f => new Book()
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(f),
                Status = Status.Ready,
                Size = f.GetFileSize(),
                FilePath = f
            }).ToList());
        }


    }
}
