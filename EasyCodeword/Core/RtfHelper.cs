using System.Windows.Forms;
using System;
using System.IO;
using System.Text;

namespace EasyCodeword.Core
{
    public static class RtfHelper
    {
        /// <summary>
        /// 从 rtf 文件中读取文本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Read(string fileName)
        {
            using (var rtb = new RichTextBox())
            {
                rtb.LoadFile(fileName);
                return rtb.Text;
            }
        }

        /// <summary>
        /// 将文本保存到文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        public static void Write(string fileName, string text)
        {
            using (var rtb = new RichTextBox())
            {
                rtb.Text = text;
                File.WriteAllText(fileName, rtb.Rtf, Encoding.Default);
            }
        }
    }
}
