using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utlity.Common;

namespace KindleConverter_WPF.Models
{
    public class Book : NotificationObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
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
        public string FilePath { get; set; }
    }
}
