using DataContract;
using DownloadManager.Dataflow;
using HtmlAgility;
using HtmlDowload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Utility.Common;
using Utility.Logging;

namespace DownloadManager
{
    public abstract class BtDataFlowBase: IManager
    {
        public string FavoriatePath {get; set;}
        public string LocalPath { get; set; }
        public DataflowService Service { get; set; }
        protected TransformBlock<DownloadAction, DownloadAction> startStep;
        protected ActionBlock<DownloadAction> endStep;
       
        public BtDataFlowBase(DataflowService service)
        {
            this.Service = service;
            ConfigDataflow();
        }

        public virtual void Execute()
        {
            List<DownloadAction> source = Service.GetBookmark(FavoriatePath);

            foreach (var action in source)
            {
                startStep.Post(action);
            }

            // Mark the head of the pipeline as complete. The continuation tasks  
            // propagate completion through the pipeline as each part of the  
            // pipeline finishes.
            startStep.Complete();

            // Wait for the last block in the pipeline to process all messages.
            endStep.Completion.Wait();

        }

        protected virtual void ConfigDataflow()
        {
            var downloadLinkBlock = Block.GetTransformBlockBlock((action) => Service.DownloadLink(action));
            var downloadFileBlock = Block.GetTransformBlockBlock((action) => Service.DownloadFile(action));
            var removeFavoriteBlock = Block.GetActionBlock((action) => Service.RemoveFavorite(action));

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

            startStep = downloadLinkBlock;
            endStep = removeFavoriteBlock;

        }
    }
}
