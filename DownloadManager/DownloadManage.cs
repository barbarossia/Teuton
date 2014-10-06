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
using HtmlAgility;
using HtmlDowload;
using BaiduNetDisk;
using DataContract;
using BaiduNetDisk.ExceptionHandler;
using DownloadManager.Dataflow;

namespace DownloadManager
{
    public class DownloadManage: BtDataFlowBase, IManager
    {
        public string RemotePath { get; set; }

        public DownloadManage(DataflowService service)
            : base(service)
        {
        }

        protected override void ConfigDataflow()
        {
            var saveBaiduNetDiskBlock = Block.GetTransformBlockBlock((action) => Service.SaveBaiduNetDisk(action));
            var removeFileBlock = Block.GetActionBlock((action) =>
            {
                if (action.CanDelete)
                    action.TempFile.DeleteFile();
            });

            saveBaiduNetDiskBlock.LinkTo(removeFileBlock);

            saveBaiduNetDiskBlock.Completion.ContinueWith(t => removeFileBlock.Complete());

            startStep = saveBaiduNetDiskBlock;
            endStep = removeFileBlock;
        }

        public override void Execute()
        {
            Service.ClearTask();

            var files = LocalPath.GetFiles();
            var actions = files.Select(f => new DownloadAction()
            {
                TempFile = f,
                CanDelete = false,
                TorrentPath = RemotePath + "/" + f.GetFileName(),
                SavePath = RemotePath,
            });

            foreach (var action in actions)
            {
                startStep.Post(action);
            }

            startStep.Complete();

            // Wait for the last block in the pipeline to process all messages.
            endStep.Completion.Wait();

            //service.QueryBaiduNetDisk();
        }
    }
}
