using DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlDowload
{
    public interface IDownloader
    {
        string Download(DownloadContent file);
        string[] Download(List<DownloadContent> file);
    }
}
