using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Utility.Common;

namespace DownloadManager.Dataflow
{
    public static class Block
    {
        public static TransformBlock<DownloadAction, DownloadAction> GetTransformBlockBlock(Action<DownloadAction> action)
        {
            return new TransformBlock<DownloadAction, DownloadAction>(a =>
            {
                return ExceptionUtility.IgnorException<DownloadAction>(() =>
                {
                    action(a);
                    return a;
                });
            });
        }

        public static ActionBlock<DownloadAction> GetActionBlock(Action<DownloadAction> func)
        {
            return new ActionBlock<DownloadAction>(a =>
            {
                ExceptionUtility.IgnorException<bool>(() =>
                {
                    func(a);
                    return true;
                });
            });

        }
    }
}
