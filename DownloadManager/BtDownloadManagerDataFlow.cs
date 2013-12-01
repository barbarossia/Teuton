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
    public class BtDownloadManagerDataFlow
    {
        private string accessToken = ConfigManager.AppSettings["AccessToken"];
        private const string root = "/apps/PCS_API01/";
        private const string name = "Bttiantang";
        private const string path = root + name + "/";
        private const string torrent = path;
        private ISearch search;
        private IDownloader download;
        private BaiduClient client;
        private TransformBlock<DownloadAction, DownloadAction> downloadLinkBlock;
        private TransformBlock<DownloadAction, DownloadAction> downloadFileBlock;
        private ActionBlock<DownloadAction> removeFavoriteBlock;
        private DataflowService service;

        public BtDownloadManagerDataFlow()
        {
            search = new BttiantangRepository();
            download = new BttiantangDownload();
            client = new BaiduClient(accessToken);
            service = new DataflowService()
            {
                Client = client,
                Download = download,
                Search = search,
                Logger = NullLogger.Instance,
                TorrentPath = torrent,
                SavePath = path,
            };

            ConfigDataflow();
        }

        public void Execute()
        {
            List<DownloadAction> source = service.GetBookmark(name);

            foreach (var action in source)
            {
                downloadLinkBlock.Post(action);
            }

            // Mark the head of the pipeline as complete. The continuation tasks  
            // propagate completion through the pipeline as each part of the  
            // pipeline finishes.
            downloadLinkBlock.Complete();

            // Wait for the last block in the pipeline to process all messages.
            removeFavoriteBlock.Completion.Wait();

        }

        private void ConfigDataflow()
        {
            downloadLinkBlock = Block.GetTransformBlockBlock((action) => service.DownloadLink(action));
            downloadFileBlock = Block.GetTransformBlockBlock((action) => service.DownloadFile(action));
            removeFavoriteBlock = Block.GetActionBlock((action) => service.RemoveFavorite(action));

            downloadLinkBlock.LinkTo(downloadFileBlock);
            downloadFileBlock.LinkTo(removeFavoriteBlock);

            // 
            // For each completion task in the pipeline, create a continuation task 
            // that marks the next block in the pipeline as completed. 
            // A completed dataflow block processes any buffered elements, but does 
            // not accept new elements. 
            //

            downloadLinkBlock.Completion.ContinueWith(t => downloadFileBlock.Complete());
            downloadFileBlock.Completion.ContinueWith(t => removeFavoriteBlock.Complete());
        }

    }
}
