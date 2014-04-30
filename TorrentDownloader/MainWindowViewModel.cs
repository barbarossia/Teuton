using Magnet2Bt;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TorrentDownloader.Models;
using Utility.Common;
using Utility.IO;
using Utility.Progress;
using Utility.Web;

namespace TorrentDownloader
{
    public class MainWindowViewModel : NotificationObject
    {
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DownloadCommand { get; set; }
        public Window View { get; set; }
        private ObservableCollection<HashObject> hashObjects = new ObservableCollection<HashObject>();
        private HashObject selectedItem;
        private string TorrentDirectory = ConfigurationManager.AppSettings["Torrent"];
        private string hash;

        public string Hash
        {
            get
            {
                return hash;
            }
            set
            {
                hash = value;
                RaisePropertyChanged(() => Hash);
            }
        }

        public ObservableCollection<HashObject> HashObjects
        {
            get
            {
                return hashObjects;
            }
            set
            {
                hashObjects = value;
                RaisePropertyChanged(() => HashObjects);
            }
        }

        public HashObject SelectedItem
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

        public MainWindowViewModel()
        {
            AddCommand = new DelegateCommand(AddFile);
            DownloadCommand = new DelegateCommand(DownloadFiles);
        }

        private void AddFile()
        {
            HashObjects.Add(new HashObject()
            {
                Hash = Hash,
                Status = Status.Ready,
            });

            Hash = string.Empty;
        }

        private void DownloadFiles()
        {
            foreach (var hash in HashObjects)
            {
                string url = MBConverter.Convert(hash.Hash);
                var progress = new Progress<TaskAsyncProgress>();
                WebUtility client = new WebUtility(null, null, progress);
                progress.ProgressChanged += progress_ProgressChanged;
                client.DownloadFileAsync(url, TorrentDirectory);
            }
        }

        private void progress_ProgressChanged(object sender, TaskAsyncProgress e)
        {
            HashObject hashObject = HashObjects.FirstOrDefault(b => b.Hash == e.Key);
            if (hashObject != null)
            {
                hashObject.Status = (Status)e.Statue;
            }  
        }
    }
}
