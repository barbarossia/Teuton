using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var btmanager = new BtDownloadManagerDataFlow();
            var manager = new DownloadManage();
            btmanager.Execute();
            manager.Execute();
        }
    }
}
