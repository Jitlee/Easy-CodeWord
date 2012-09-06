using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    internal static class PowerHelper
    {
        
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;
        private static bool DoExitWin(int flg)
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = NativeMethods.GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = NativeMethods.OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = NativeMethods.LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = NativeMethods.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ok = NativeMethods.ExitWindowsEx(flg, 0);
            return ok;
        }

        public static void Logoff()
        {
            DoExitWin(EWX_LOGOFF);
        }
        public static void Reboot()
        {
            DoExitWin(EWX_REBOOT);
        }

        public static void Shutdown()
        {
            DoExitWin(EWX_SHUTDOWN);
        }

        public static void Poweroff()
        {
            DoExitWin(EWX_POWEROFF); ;
        }
    }
}
