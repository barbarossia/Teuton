using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace KindleConverter_WPF.Models
{
    public class Book : NotificationObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
        private BookStatus state;
        public Status Status{ get; set; }

        public BookStatus State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                RaisePropertyChanged(() => State);
            }
        }
        public string FilePath { get; set; }
        public string MobiFilePath { get; set; }
        public string TempFilePath { get; set; }
    }
}
