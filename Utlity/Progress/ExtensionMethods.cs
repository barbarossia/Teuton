using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Progress
{
    public static class ExtensionMethods
    {
        public static void ReportStatus(
            this IProgress<TaskAsyncProgress> progress, 
            object sender,
            string key, 
            object status)
        {
            if (progress != null)
            {
                TaskAsyncProgress report = new TaskAsyncProgress()
                {
                    Sender = sender,
                    Key = key,
                    Statue = status
                };
                progress.Report(report);
            }
        }

        public static void ReportStatus(
            this IProgress<TaskAsyncProgress> progress,
            string key,
            object status)
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
