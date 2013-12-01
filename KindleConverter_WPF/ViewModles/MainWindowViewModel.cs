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
using Utility.Common;
using Utility.IO;
using Utility.Progress;

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
        private CancellationTokenSource cancellationToken;
        private BookTransfer transfer;
        private Dataflow dataflow;

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

            transfer = new BookTransfer(
                null,
                 (e) => this.progress_ProgressChanged(null, e),
                 cancellationToken);

            try
            {
                dataflow = new Dataflow(
                    books,
                    transfer,
                    () => this.OnCompleted());

                await dataflow.Execute();
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

        private void progress_ProgressChanged(object sender, TaskAsyncProgress e)
        {
            Book book = this.Books.FirstOrDefault(b => b.Name == e.Key);
            if (book != null)
            {
                book.Status = (Status)e.Statue;
                if (book.Status == Status.Progressing || book.Status == Status.Completed)
                {
                    if (e.Sender.GetType() == typeof(EpubConverter) && book.Status == Status.Progressing)
                    {
                        book.State = BookStatus.Converting;
                    }
                    else if (e.Sender.GetType() == typeof(EpubConverter) && book.Status == Status.Completed)
                    {
                        book.State = BookStatus.Converted;
                    }
                    else if (e.Sender.GetType() == typeof(GMail) && book.Status == Status.Progressing)
                    {
                        book.State = BookStatus.Sending;
                    }
                    else if (e.Sender.GetType() == typeof(GMail) && book.Status == Status.Completed)
                    {
                        book.State = BookStatus.Completed;
                    }
                }
                else
                {
                    switch(book.Status)
                    {
                        case Status.Canceled:
                           book.State = BookStatus.Canceled;
                            break;
                        case Status.Error:
                           book.State = BookStatus.Error;
                            break;
                        case Status.Failed:
                            book.State = BookStatus.Failed;
                            break;
                        case Status.Ready:
                            book.State = BookStatus.Ready;
                            break;
                    }
                }
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
