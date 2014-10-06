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
                SavePath = SavePath,
            }).ToList();
        }

        public void DownloadLink(DownloadAction action)
        {
            var content = Search.Search(action.Url);
            action.Content = content;
        }

        public void DownloadFile(DownloadAction action)
        {
            try
            {
                var tempFile = Download.Download(action.Content);
                tempFile.FileCopyToDirctory(SavePath);
                action.CanDelete = true;
            }
            catch (Exception ex)
            {
                Logger.Log(MessageLevel.Error, ex.Message, ex);
            }
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
            if (tasks.Any())
            {
                var details = Client.QueryTaskDetail(tasks);
            }
        }

        public  void ClearTask()
        {
            var tasks = Client.QueryTaskList();
            if (tasks.Any())
            {
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
        }

        private bool CanCancelDownloadingTask(NetTaskInfo task)
        {
            return CheckDownloadingTaskFinished(task.FinishedSize, task.FinishTime) ||
                CheckDownloadingTaskRate(task.FinishedSize, task.TotalSize);
        }

        private bool CheckDownloadingTaskRate(long finished, long total)
        {
            return ((double)finished / (double)total) > 0.99;
        }

        private bool CheckDownloadingTaskFinished(long finished, DateTime finishedTime)
        {
            DateTime end = new DateTime(1970,1,1);
            return finished == 0 && finishedTime > end;
        }
    }
}
