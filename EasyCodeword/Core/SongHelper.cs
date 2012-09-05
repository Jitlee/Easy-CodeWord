using System;	
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace EasyCodeword.Core
{
    public static class SongHelper
    {
        private const int SIZE = 128;
        // 获取MP3文件最后128个字节
        /// <summary>
        /// 获取MP3文件最后128个字节
        /// </summary>
        /// <param name= "fileName">文件名 </param>
        /// <returns> 返回字节数组 </returns>
        private static byte[] GetLast128(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileStream.Seek(-SIZE, SeekOrigin.End);
                byte[] info = new byte[SIZE];
                fileStream.Read(info, 0, SIZE);
                return info;
            }
        }

        // 取MP3歌曲的相关信息
        /// <summary>
        /// 取MP3歌曲的相关信息
        /// </summary>
        /// <param   name   =   "Info "> 从MP3文件中截取的二进制信息 </param>
        /// <returns> 返回一个Mp3Info结构 </returns>
        public static SongInfo GetMp3Info(string fileName)
        {
            var songInfo = new SongInfo();
            try
            {
                var buffer = GetLast128(fileName);
                if (buffer.Length == SIZE)
                {
                    var index = 0;
                    songInfo.Identify = Encoding.Default.GetString(buffer, index, 3); index += 3;
                    songInfo.Title = Encoding.Default.GetString(buffer, index, 30).TrimEnd('\0'); index += 30;
                    songInfo.Artist = Encoding.Default.GetString(buffer, index, 30).TrimEnd('\0'); index += 30;
                    songInfo.Album = Encoding.Default.GetString(buffer, index, 30).TrimEnd('\0'); index += 30;
                    songInfo.Year = Encoding.Default.GetString(buffer, index, 4).TrimEnd('\0'); index += 4;
                    songInfo.Comment = Encoding.Default.GetString(buffer, index, 28).TrimEnd('\0'); index += 28;
                }
            }
            catch
            {
            }
            return songInfo;
        }
    }

    public struct SongInfo
    {
        /// <summary>
        /// TAG
        /// </summary>
        public string Identify;//TAG，三个字节
        /// <summary>
        /// 歌曲名
        /// </summary>
        public string Title;//歌曲名,30个字节
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist;//歌手名,30个字节
        /// <summary>
        /// 唱片
        /// </summary>
        public string Album;//所属唱片,30个字节
        /// <summary>
        /// 年代
        /// </summary>
        public string Year;//年,4个字符
        /// <summary>
        /// 备注
        /// </summary>
        public string Comment;//注释,28个字节
        public char reserved1;//保留位，一个字节
        public char reserved2;//保留位，一个字节
        public char reserved3;//保留位，一个字节
    }
}