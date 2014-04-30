using BaiduNetDisk;
using DataContract;
using HtmlAgility;
using HtmlDowload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Web;
using Utility.IO;
using BaiduNetDisk.DataContracts;
using System.Configuration;
using Utility.Common;
using Utility.Logging;
using System.Threading.Tasks.Dataflow;
using BaiduNetDisk.ExceptionHandler;
using DownloadManager.Dataflow;

namespace DownloadManager
{
    public class BtDownloadManagerDataFlow : BtDataFlowBase, IManager
    {
        public BtDownloadManagerDataFlow(DataflowService service)
            : base(service)
        {
        }
    }
}
