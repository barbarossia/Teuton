using HtmlAgility;
using HtmlDowload;
using Ninject.Modules;
using Utility.Logging;


namespace DownloadManager
{
    public class YyModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISearch>().To<YyetsRepository>();
            Bind<IDownloader>().To<YytesDownload>();
            Bind<ISearchResultSelector>().To<YytesSelector>();
            Bind<ILogger>().ToConstant(NullLogger.Instance);
        }
    }
}
