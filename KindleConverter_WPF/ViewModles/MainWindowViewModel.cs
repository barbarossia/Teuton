using KindleConverter_WPF.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utlity.Common;
using Utlity.Progress;
using Mail;
using KindelConverter;
using System.Windows.Threading;
using System.Threading;

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
                    string convertedPath = System.IO.Path.ChangeExtension(currentItem.FilePath, ".mobi");
                    mail.SendMailAsync(convertedPath, cancellationToken.Token);
                }
            }
        }

        private void progress_ProgressChanged(object sender, TaskAsyncProgress e)
        {
            Book book = this.Books.FirstOrDefault(b => b.Name == e.Key);
            if (book != null)
            {
                book.Status = (Status)e.Statue;
            }
        }

        private void BrowseFile()
        {
            var fileNames = ShowOpenFileDialogAndReturnResult("Epub files (*.epub)|*.epub", "Open Epub File");

            Books = new List<Book>(fileNames.Select(f => new Book()
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(f),
                Status = Status.Ready,
                Size = GetFileSize(f),
                FilePath = f
            }).ToList());
        }

        private long GetFileSize(string fileName)
        {
            FileInfo f = new FileInfo(fileName);
            return f.Length;
        }

        public string[] ShowOpenFileDialogAndReturnResult(string filter, string dialogTitle)
        {
            string[] fileNames = new string[] { };
            var openFileDialog = CreateOpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = true;

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = "Open";
            }

            openFileDialog.Title = dialogTitle;
            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileNames = openFileDialog.FileNames;
            }

            return fileNames;
        }

        public Func<OpenFileDialog> CreateOpenFileDialog = () => new OpenFileDialog();
    }
}
