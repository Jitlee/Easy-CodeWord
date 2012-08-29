using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace EasyCodeword.Utilities
{
    internal class RSA
    {
        // 生成公私钥
        /// <summary>
        /// 生成公私钥
        /// </summary>
        /// <param name="PrivateKeyPath"></param>
        /// <param name="privateKeyPath"></param>
        internal static void RSAKey(string PrivateKeyPath, string privateKeyPath)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                CreatePrivateKeyXML(PrivateKeyPath, provider.ToXmlString(true));
                CreateprivateKeyXML(privateKeyPath, provider.ToXmlString(false));
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        // 对原始数据进行MD5加密
        /// <summary>
        /// 对原始数据进行MD5加密
        /// </summary>
        /// <param name="m_strSource">待加密数据</param>
        /// <returns>返回机密后的数据</returns>
        internal static string GetHash(string m_strSource)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create("MD5");
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            byte[] inArray = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }

        // RSA加密
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlprivateKey">公钥</param>
        /// <param name="m_strEncryptString">MD5加密后的数据</param>
        /// <returns>RSA公钥加密后的数据</returns>
        internal static string RSAEncrypt(string xmlprivateKey, string m_strEncryptString)
        {
            string str2;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlprivateKey);
                byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
                str2 = Convert.ToBase64String(provider.Encrypt(bytes, false));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }

        // RSA解密
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="m_strDecryptString">待解密的数据</param>
        /// <returns>解密后的结果</returns>
        internal static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            string str2;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPrivateKey);
                byte[] rgb = Convert.FromBase64String(m_strDecryptString);
                byte[] buffer2 = provider.Decrypt(rgb, false);
                str2 = new UnicodeEncoding().GetString(buffer2);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }

        // 对MD5加密后的密文进行签名
        /// <summary>
        /// 对MD5加密后的密文进行签名
        /// </summary>
        /// <param name="p_strKeyPrivate">私钥</param>
        /// <param name="m_strHashbyteSignature">MD5加密后的密文</param>
        /// <returns></returns>
        internal static string SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature)
        {
            byte[] rgbHash = Convert.FromBase64String(m_strHashbyteSignature);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("MD5");
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
        }

        // 签名验证
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="p_strKeyprivate">公钥</param>
        /// <param name="p_strHashbyteDeformatter">待验证的用户名</param>
        /// <param name="p_strDeformatterData">注册码</param>
        /// <returns></returns>
        internal static bool SignatureDeformatter(string p_strKeyprivate, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            try
            {
                byte[] rgbHash = Convert.FromBase64String(p_strHashbyteDeformatter);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(p_strKeyprivate);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("MD5");
                byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // 创建公钥文件
        /// <summary>
        /// 创建公钥文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="privatekey"></param>
        internal static void CreateprivateKeyXML(string path, string privatekey)
        {
            try
            {
                FileStream privatekeyxml = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(privatekeyxml);
                sw.WriteLine(privatekey);
                sw.Close();
                privatekeyxml.Close();
            }
            catch
            {
                throw;
            }
        }

        // 创建私钥文件
        /// <summary>
        /// 创建私钥文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="privatekey"></param>
        internal static void CreatePrivateKeyXML(string path, string privatekey)
        {
            using(FileStream privatekeyxml = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(privatekeyxml))
                {
                    sw.WriteLine(privatekey);
                }
            }
        }

        // 读取公钥
        /// <summary>
        /// 读取公钥
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string ReadprivateKey(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string privatekey = reader.ReadToEnd();
                reader.Close();
                return privatekey;
            }
        }

        // 读取私钥
        /// <summary>
        /// 读取私钥
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string ReadPrivateKey(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string privatekey = reader.ReadToEnd();
                reader.Close();
                return privatekey;
            }
        }

        // 初始化注册表，程序运行时调用，在调用之前更新公钥xml
        /// <summary>
        /// 初始化注册表，程序运行时调用，在调用之前更新公钥xml
        /// </summary>
        /// <param name="path">公钥路径</param>
        internal static void InitialReg(string path)
        {
            //Registry.LocalMachine.CreateSubKey(@"SOFTWARE/JX/Register");
            //Random ra = new Random();
            //string privatekey = this.ReadprivateKey(path);
            //if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE/JX/Register").ValueCount <= 0)
            //{
            //    this.WriteReg("RegisterRandom", ra.Next(1, 100000).ToString());
            //    this.WriteReg("RegisterprivateKey", privatekey);
            //}
            //else
            //{
            //    this.WriteReg("RegisterprivateKey", privatekey);
            //}
        }
    }
}
