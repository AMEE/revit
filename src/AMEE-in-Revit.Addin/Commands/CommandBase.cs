using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AMEE_in_Revit.Addin.Commands
{
    public class CommandBase
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string lpszClass,
            string lpszWindow);

        public static void SetStatusText(string text, params object[] args)
        {
            var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
            var statusBar = FindWindowEx(mainWindow, IntPtr.Zero,"msctls_statusbar32", "");

            if (statusBar != IntPtr.Zero)
            {
                SetWindowText(statusBar, string.Format(text, args));
            }
        }
    }
}