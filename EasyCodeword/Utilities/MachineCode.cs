using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyCodeword.Utilities
{
    internal static class MachineCode
    {
        // 读取 CPU 序列号
        /// <summary>
        /// 读取 CPU 序列号
        /// </summary>
        /// <returns></returns>
        internal static string GetCpuID()
        {
            try
            {
                using (ManagementObjectCollection collection1 = new ManagementClass("Win32_Processor").GetInstances())
                {
                    var cupID = string.Empty;
                    foreach (ManagementObject obj1 in collection1)
                    {
                        cupID = obj1.Properties["ProcessorId"].Value.ToString();
                        break;
                    }
                    return cupID;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        //读取硬盘序列号
        /// <summary>
        /// 读取硬盘序列号
        /// </summary>
        /// <returns></returns>
        internal static string GetHardDiskID()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia"))
                {
                    var hardDiskID = string.Empty;
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        hardDiskID = mo["SerialNumber"].ToString().Trim();
                        break;
                    }
                    return hardDiskID;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        #region 获取网卡 MAC 地址

        // 取网卡 MAC 地址
        /// <summary>
        /// 取网卡 MAC 地址
        /// </summary>
        /// <returns></returns>
        internal static string GetMacAddress()
        {
            string addr = string.Empty;
            int cb;
            ASTAT adapter;
            NCB ncb = new NCB();
            char uRetCode;
            LANA_ENUM lenum;
            try
            {
                ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                cb = Marshal.SizeOf(typeof(LANA_ENUM));
                ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                ncb.ncb_length = (ushort)cb;
                uRetCode = NativeMethods.Netbios(ref ncb);
                lenum = (LANA_ENUM)Marshal.PtrToStructure(ncb.ncb_buffer, typeof(LANA_ENUM));
                Marshal.FreeHGlobal(ncb.ncb_buffer);
                if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                    return "";
                for (int i = 0; i < lenum.length; i++)
                {
                    ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                    ncb.ncb_lana_num = lenum.lana[i];
                    uRetCode = NativeMethods.Netbios(ref ncb);
                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";
                    ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                    ncb.ncb_lana_num = lenum.lana[i];
                    ncb.ncb_callname[0] = (byte)'*';
                    cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) + Marshal.SizeOf(typeof(NAME_BUFFER)) * (int)NCBCONST.NUM_NAMEBUF;
                    ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    ncb.ncb_length = (ushort)cb;
                    uRetCode = NativeMethods.Netbios(ref ncb);
                    adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                    Marshal.FreeHGlobal(ncb.ncb_buffer);
                    if (uRetCode == (short)NCBCONST.NRC_GOODRET)
                    {
                        if (i > 0)
                            addr += ":";
                        addr = string.Format("{0,2:X}{1,2:X}{2,2:X}{3,2:X}{4,2:X}{5,2:X}",
                        adapter.adapt.adapter_address[0],
                        adapter.adapt.adapter_address[1],
                        adapter.adapt.adapter_address[2],
                        adapter.adapt.adapter_address[3],
                        adapter.adapt.adapter_address[4],
                        adapter.adapt.adapter_address[5]);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                ncb.Dispose();
            }
            return addr.Replace(' ', '0');
        }

        #endregion
    }
}
