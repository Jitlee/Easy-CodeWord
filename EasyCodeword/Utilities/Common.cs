using System;
using System.IO;
using System.Runtime.InteropServices;

namespace EasyCodeword.Utilities
{
    public class Common
    {
        public readonly static string ApplicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasyCodeword");

        public readonly static string TempFile = GetAppPath("tmp");

        public static string GetAppPath(string path)
        {
            return Path.Combine(ApplicationDataPath, path);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x00010000;
        const int WS_MINIMIZEBOX = 0x00020000;

        public static void DisableMinmize(IntPtr hWnd)
        {
            SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_MINIMIZEBOX & ~WS_MAXIMIZEBOX);
        }
    }
}
