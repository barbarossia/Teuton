using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace TorrentDownloader.Models
{
    public class HashObject : NotificationObject
    {
        public string Hash { get; set; }
        private Status status;
        public Status Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                RaisePropertyChanged(() => Status);
            }
        }
    }
}
