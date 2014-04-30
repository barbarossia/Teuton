using DownloadManager.Dataflow;
using HtmlAgility;
using HtmlDowload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Logging;

namespace DownloadManager
{
    public class YyDownloadManagerDataFlow : BtDataFlowBase, IManager
    {
        public YyDownloadManagerDataFlow(DataflowService service)
            : base(service)
        {
        }
    }
}
