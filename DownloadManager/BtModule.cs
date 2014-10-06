using HtmlAgility;
using HtmlDowload;
using Ninject.Modules;
using Utility.Logging;

namespace DownloadManager
{
    public class BtModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISearch>().To<BttiantangRepository>();
            Bind<IDownloader>().To<BttiantangDownload>();
            Bind<ISearchResultSelector>().To<BttiantangSelector>();
            Bind<ILogger>().ToConstant(NullLogger.Instance);
        }
    }
}
