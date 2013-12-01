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
    public class DownloadManage
    {
        private string accessToken = ConfigManager.AppSettings["AccessToken"];
        private const string root = "/apps/PCS_API01/";
        private const string name = "Bttiantang";
        private const string path = root + name + "/";
        private const string torrent = path;
        private ILogger logger = NullLogger.Instance;
        private BaiduClient client;
        private DataflowService service;
        private const string dir = @"C:\Users\v-bobzh\Documents\Torrent";

        public DownloadManage()
        {
            client = new BaiduClient(accessToken);
            service = new DataflowService()
            {
                Client = client,
                Logger = NullLogger.Instance,
            };
        }

        public void Execute()
        {
            try
            {
                service.ClearTask();

                var saveBaiduNetDiskBlock = Block.GetTransformBlockBlock((action) => service.SaveBaiduNetDisk(action));
                var removeFileBlock = Block.GetActionBlock((action) =>
                {
                    if (action.CanDelete)
                        action.TempFile.DeleteFile();
                });

                saveBaiduNetDiskBlock.LinkTo(removeFileBlock);

                saveBaiduNetDiskBlock.Completion.ContinueWith(t => removeFileBlock.Complete());

                var files = dir.GetFiles();
                var actions = files.Select(f => new DownloadAction()
                {
                    TempFile = f,
                    CanDelete = false,
                    TorrentPath = torrent + f.GetFileName(),
                    SavePath = path,
                });

                foreach (var action in actions)
                {
                    saveBaiduNetDiskBlock.Post(action);
                }

                saveBaiduNetDiskBlock.Complete();

                // Wait for the last block in the pipeline to process all messages.
                removeFileBlock.Completion.Wait();

                service.QueryBaiduNetDisk();
            }
            catch (Exception ex)
            {
                logger.Log(MessageLevel.Error, "error", ex);
            }
        }
    }
}
