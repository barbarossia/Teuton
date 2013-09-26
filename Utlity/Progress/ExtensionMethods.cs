using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utlity.Progress
{
    public static class ExtensionMethods
    {
        public static void ReportStatus(this IProgress<TaskAsyncProgress> progress, string key, object status)
        {
            if (progress != null)
            {
                TaskAsyncProgress report = new TaskAsyncProgress()
                {
                    Key = key,
                    Statue = status
                };
                progress.Report(report);
            }
        }
    }
}
