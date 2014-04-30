using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AccessToken
{
    public class Launcher
    {
        [DllImport("user32.dll")]
        private extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        const int SWP_SHOWWINDOW = 0x0040;
        static readonly IntPtr HWND_TOP = IntPtr.Zero;

        public static void Launch(string url)
        {
            Process p1 = new Process();
            p1.StartInfo.FileName = "iexplore.exe";
            p1.StartInfo.Arguments = url;
            p1.Start();
            p1.WaitForInputIdle(2000);
            System.Threading.Thread.Sleep(2000);

            //Rectangle monitor = Screen.AllScreens[0].WorkingArea;
            //SetWindowPos(p1.MainWindowHandle, HWND_TOP, monitor.Left, monitor.Top, monitor.Width, monitor.Height, SWP_SHOWWINDOW);


            //Process p2 = new Process();
            //p2.StartInfo.FileName = "iexplore.exe";
            //p2.StartInfo.Arguments = "google.com";
            //p2.Start();
            //p2.WaitForInputIdle(2000);
            //System.Threading.Thread.Sleep(2000);

            //Rectangle monitor2 = Screen.AllScreens[1].WorkingArea;
            //SetWindowPos(p2.MainWindowHandle, HWND_TOP, monitor2.Left, monitor2.Top, monitor2.Width, monitor2.Height, SWP_SHOWWINDOW);
        }


    }
}
