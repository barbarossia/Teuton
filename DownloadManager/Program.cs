using BaiduNetDisk;
using DownloadManager.Dataflow;
using HtmlAgility;
using HtmlDowload;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Logging;

namespace DownloadManager
{
    class Program
    {
        static string localPath = ConfigManager.AppSettings["LocalPath"];
        static string favoriatePath = ConfigManager.AppSettings["FavoriatePath"];
        static string favoriatePath2 = ConfigManager.AppSettings["FavoriatePath2"];
        static string accessToken = ConfigManager.AppSettings["AccessToken"];
        static string remotePath = ConfigManager.AppSettings["RemotePath"];

        static void Main(string[] args)
        {
            var btmanager = BindBtManager();
            var yyManager = BindYyManager();
            //var manager = BindDownloadManager();
            btmanager.Execute();
            yyManager.Execute();
            //manager.Execute();
        }

        static IManager BindBtManager()
        {
            using (var kernal = new StandardKernel(new BtModule()))
            {
                var service = new DataflowService()
                {
                    Download = kernal.Get<IDownloader>(),
                    Search = kernal.Get<ISearch>(),
                    Logger = kernal.Get<ILogger>(),
                    SavePath = localPath,
                };

                service.Search.Selector = kernal.Get<ISearchResultSelector>();

                kernal.Bind<IManager>().To<BtDownloadManagerDataFlow>()
                    .WithConstructorArgument("service", service)
                    .WithPropertyValue("FavoriatePath", favoriatePath);
                return kernal.Get<IManager>();
            }
        }

        static IManager BindYyManager()
        {
            using (var kernal = new StandardKernel(new YyModule()))
            {
                var service = new DataflowService()
                {
                    Download = kernal.Get<IDownloader>(),
                    Search = kernal.Get<ISearch>(),
                    Logger = kernal.Get<ILogger>(),
                    SavePath = localPath,
                };

                service.Search.Selector = kernal.Get<ISearchResultSelector>();

                kernal.Bind<IManager>().To<YyDownloadManagerDataFlow>()
                    .WithConstructorArgument("service", service)
                    .WithPropertyValue("FavoriatePath", favoriatePath2);
                return kernal.Get<IManager>();
            }
        }

        static IManager BindDownloadManager()
        {
            using (var kernal = new StandardKernel(new BtModule()))
            {
                var client = new BaiduClient(accessToken);
                var service = new DataflowService()
                {
                    Client = client,
                    Logger = kernal.Get<ILogger>(),
                };

                kernal.Bind<IManager>().To<DownloadManage>()
                    .WithConstructorArgument("service", service)
                    .WithPropertyValue("LocalPath", localPath)
                    .WithPropertyValue("RemotePath", remotePath);
                return kernal.Get<IManager>();
            }
        }
    }
}
