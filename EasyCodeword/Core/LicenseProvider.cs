using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    internal static class LicenseProvider
    {
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(LicenseProvider).FullName);

        // 获取特征码
        /// <summary>
        /// 获取特征码
        /// </summary>
        internal static readonly string Code = GetCode();

        // 序列号
        /// <summary>
        /// 序列号
        /// </summary>
        internal static string SerialNumber { get; private set; }

        // 获取软件是否注册
        /// <summary>
        /// 获取软件是否注册
        /// </summary>
        internal static bool IsRegistered { get; private set; }

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
            return i.ToString().PadLeft(19, '8');
        }

        private static string GetGenerateSerialNumber(string code)
        {
            return MD5(AES.Encrypt(code, Constants.Seed));
        }

        /// <summary>
        /// 格式化序列号
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        internal static string FormatSerialNumber(string serialNumber)
        {
            serialNumber = serialNumber.Replace("-", "");
            if (serialNumber.Length == 16)
            {
                serialNumber = serialNumber.Insert(12, "-");
                serialNumber = serialNumber.Insert(8, "-");
                serialNumber = serialNumber.Insert(4, "-");
            }
            return serialNumber;
        }

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

        /// <summary>
        /// 验证是否已注册
        /// </summary>
        public static bool Verify()
        {
            IsRegistered = false;
            try
            {
                SerialNumber = RWReg.GetValue(Microsoft.Win32.Registry.LocalMachine,
                    Constants.SubName,
                    "Cache", string.Empty).ToString().Replace("-", "");

                if (!string.IsNullOrEmpty(SerialNumber))
                {
                    var number = GetGenerateSerialNumber(Code);
                    IsRegistered = string.Equals(SerialNumber, number, StringComparison.InvariantCultureIgnoreCase);
                    return IsRegistered;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[Verify] Exception : {0}", ex.Message);
            }
            return false;
        }
    }
}
