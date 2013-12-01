using BaiduNetDisk;
using BaiduNetDisk.DataContracts;
using BaiduNetDisk.ExceptionHandler;
using DataContract;
using HtmlAgility;
using HtmlDowload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Logging;
using Utility.Web;
using Utility.IO;

namespace DownloadManager.Dataflow
{
    public class DataflowService
    {
        private const string dir = @"C:\Users\v-bobzh\Documents\Torrent";

        public ILogger Logger { get; set; }
        public ISearch Search { get; set; }
        public IDownloader Download { get; set; }
        public BaiduClient Client { get; set; }
        public string TorrentPath { get; set; }
        public string SavePath { get; set; }

        public  List<DownloadAction> GetBookmark(string source)
        {
            var Files = Bookmark.GetIEFavoritesUrl(source);
            return Files.Select(f => new DownloadAction()
            {
                Path = f,
                Url = Bookmark.GetUrl(f),
                CanDelete = false,
                TorrentPath = TorrentPath,
                SavePath = SavePath,
            }).ToList();
        }

        public  void DownloadLink(DownloadAction action)
        {
            var content = Search.Search(action.Url);
            action.Content = content;
        }

        public  void DownloadFile(DownloadAction action)
        {
            var tempFile = Download.Download(action.Content);
            tempFile.FileCopyToDirctory(dir);
            //action.TempFile = tempFile;
            action.CanDelete = true;
        }

        public  void SaveBaiduNetDisk(DownloadAction action)
        {
            try
            {
                try
                {
                    NetFileInfo uploadFileInfo = Client.UploadSingleFile(action.TempFile, action.TorrentPath);
                }
                catch (ServiceFileAlreadyExistsException saee)
                {
                    Logger.Log(MessageLevel.Info, saee.Message, saee);
                }
                NetTorrenInfo torrentInfo = Client.QueryTorrent(action.TorrentPath);
                NetTaskInfo taskInfo = Client.AddTorrentTask(action.TorrentPath, action.SavePath, torrentInfo);
                action.CanDelete = true;
            }
            catch (ServiceAddTaskException sae)
            {
                Logger.Log(MessageLevel.Info, sae.Message, sae);
            }
            catch (ServiceInvalidTorrent sit)
            {
                Logger.Log(MessageLevel.Info, sit.Message, sit);
                action.CanDelete = true;
            }
        }

        public  void RemoveFavorite(DownloadAction action)
        {
            if (action.CanDelete)
            {
                Bookmark.DeleteBookmark(action.Source, action.Path);
            }
        }

        public  void QueryBaiduNetDisk()
        {
            var tasks = Client.QueryTaskList();
            var details = Client.QueryTaskDetail(tasks);
        }

        public  void ClearTask()
        {
            var tasks = Client.QueryTaskList();
            var details = Client.QueryTaskDetail(tasks);
            var downloading = tasks.Where(t => t.Status == 1);
            var cancelTasks = new List<NetTaskInfo>();
            foreach (var d in downloading)
            {
                var info = details.FirstOrDefault(t => t.Name == d.Name);
                info.Id = d.Id;
                if (info != null && CanCancelDownloadingTask(info))
                    cancelTasks.Add(info);
            }

            foreach (var task in cancelTasks)
            {
                Client.CancelTask(task.Id);
            }
        }

        private bool CanCancelDownloadingTask(NetTaskInfo task)
        {
            if (task.Status == 1)
            {
                long totalSize = task.TotalSize;
                long finishSize = task.FinishedSize;
                if (finishSize == 0)
                    return false;

                if ((double)finishSize / (double)totalSize > 0.99)
                    return true;
            }
            return false;
        }
    }
}
