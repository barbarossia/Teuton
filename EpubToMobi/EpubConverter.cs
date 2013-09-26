using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utlity.Common;
using Utlity.Progress;

namespace KindelConverter
{
    public class EpubConverter
    {
        private const string PATH_KINDLEGEN = @"kindlegen.exe";
        private IProgress<TaskAsyncProgress> progess;

        public EpubConverter()
            : this(null)
        {
        }

        public EpubConverter(IProgress<TaskAsyncProgress> progressReport)
        {
            this.progess = progressReport;
        }

        public Task<bool> ConvertAsync(string path)
        {
            return Task<bool>.Run(() => Convert(path));
        }

        public bool Convert(string path)
        {
            bool result = false;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //  Set the options.
            startInfo.UseShellExecute = false;
            startInfo.ErrorDialog = false;
            startInfo.CreateNoWindow = true;

            startInfo.FileName = PATH_KINDLEGEN;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = path;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            string name = Path.GetFileNameWithoutExtension(path);

            this.progess.ReportStatus(name, Status.Converting);

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                    result = exeProcess.ExitCode == 2 ? false : true;
                }
            }
            catch
            {
                // Log error.
                //outMessage = ex.ToString();
                return false;
            }

            if (result)
                this.progess.ReportStatus(name, Status.Converted);
            else
                this.progess.ReportStatus(name, Status.ConvertedFail);
            return result;
        }
    }
}
