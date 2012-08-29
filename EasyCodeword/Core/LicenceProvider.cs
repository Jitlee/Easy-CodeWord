using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    internal static class LicenceProvider
    {
        // 获取特征码
        /// <summary>
        /// 获取特征码
        /// </summary>
        internal static readonly string Code = GetCode();

        // 获取软件是否已注册
        /// <summary>
        /// 获取软件是否已注册
        /// </summary>
        internal static bool IsRegisterd { get; private set; }

        // 序列号
        /// <summary>
        /// 序列号
        /// </summary>
        internal static string SerialNumber { get; private set; }

        // 注册机
        /// <summary>
        /// 获取机器特征码
        /// </summary>
        /// <returns></returns>
        private static string GetCode()
        {
            var cupID = MachineCode.GetCpuID();
            var hardDiskID = MachineCode.GetHardDiskID();
            var code = Encoding.ASCII.GetBytes((MD5(hardDiskID + hardDiskID)));
            long i = 1;
            foreach (byte b in code)
            {
                i *= ((int)b - 569);
            }
            return i.ToString();
        }

        //private static string GetGenerateSerialNumber(string code)
        //{

        //}

        // MD5 加密数据
        /// <summary>
        /// MD5 加密数据
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private static string MD5(string plainText)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(plainText)), 4, 8);
                t2 = t2.Replace("-", "");
                return t2;
            }
        }

        public static bool Verify()
        {
            SerialNumber = RWReg.GetValue(Microsoft.Win32.Registry.LocalMachine, Constants.SubName, "Cache", string.Empty).ToString();
            if (!string.IsNullOrEmpty(SerialNumber))
            {

            }
            return false;
        }
    }
}
