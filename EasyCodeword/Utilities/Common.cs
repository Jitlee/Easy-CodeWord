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
            if (!Directory.Exists(ApplicationDataPath))
            {
                Directory.CreateDirectory(ApplicationDataPath);
            }
            return Path.Combine(ApplicationDataPath, path);
        }

        public static string GetAppPath(string path1, string path2)
        {
            if (!Directory.Exists(ApplicationDataPath))
            {
                Directory.CreateDirectory(ApplicationDataPath);
            }
            return Path.Combine(Path.Combine(ApplicationDataPath, path1), path2);
        }

    }
}
