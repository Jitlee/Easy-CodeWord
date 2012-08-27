using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace EasyCodeword.Utilities
{
    internal class AES
    {
        //默认密钥向量 
        private static readonly byte[] DefaultKey = { 0x4f, 0xee, 0xa0, 0x99, 0x56, 0x7c, 0x8e, 0xd1, 0xaa, 0xAB, 0xF8, 0x2c, 0x94, 0xb2, 0xfa, 0x0a };

        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="plainText">明文字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>返回加密后的密文</returns>
        internal static string Encrypt(string plainText, string key)
        {
            //分组加密算法
            SymmetricAlgorithm des = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组	
            //设置密钥及密钥向量
            des.Key = ConvertKey(key);
            des.IV = DefaultKey;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组
                    var output = Convert.ToBase64String(cipherBytes);
                    cipherBytes = null;
                    return output;
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>返回解密后的字符串</returns>
        internal static string Decrypt(string cipherText, string key)
        {
            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);
                SymmetricAlgorithm des = Rijndael.Create();
                des.Key = ConvertKey(key);
                des.IV = DefaultKey;
                byte[] decryptBytes = new byte[cipherBytes.Length];
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.Read(decryptBytes, 0, decryptBytes.Length);
                        cs.Close();
                        ms.Close();
                        var output = Encoding.UTF8.GetString(decryptBytes).TrimEnd('\0');
                        decryptBytes = null;
                        cipherBytes = null;
                        return output;
                    }
                }
            }
            catch(Exception)
            {
                return cipherText;
            }
        }

        private static byte[] ConvertKey(string key)
        {
            if (null == key)
            {
                key = "©2013 icodeword";
            }
            else if (key.Length < 16)
            {
                key = key.PadLeft(16, 'k');
            }
            else if (key.Length > 16)
            {
                key = key.Remove(16);
            }
            return Encoding.UTF8.GetBytes(key);
        }
    }
}
