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

        private enum NCBCONST
        {
            NCBNAMSZ = 16, /**//* absolute length of a net name */
            MAX_LANA = 254, /**//* lana's in range 0 to MAX_LANA inclusive */
            NCBENUM = 0x37, /**//* NCB ENUMERATE LANA NUMBERS */
            NRC_GOODRET = 0x00, /**//* good return */
            NCBRESET = 0x32, /**//* NCB RESET */
            NCBASTAT = 0x33, /**//* NCB ADAPTER STATUS */
            NUM_NAMEBUF = 30, /**//* Number of NAME's BUFFER */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ADAPTER_STATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] adapter_address;
            public byte rev_major;
            public byte reserved0;
            public byte adapter_type;
            public byte rev_minor;
            public ushort duration;
            public ushort frmr_recv;
            public ushort frmr_xmit;
            public ushort iframe_recv_err;
            public ushort xmit_aborts;
            public uint xmit_success;
            public uint recv_success;
            public ushort iframe_xmit_err;
            public ushort recv_buff_unavail;
            public ushort t1_timeouts;
            public ushort ti_timeouts;
            public uint reserved1;
            public ushort free_ncbs;
            public ushort max_cfg_ncbs;
            public ushort max_ncbs;
            public ushort xmit_buf_unavail;
            public ushort max_dgram_size;
            public ushort pending_sess;
            public ushort max_cfg_sess;
            public ushort max_sess;
            public ushort max_sess_pkt_size;
            public ushort name_count;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NAME_BUFFER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] name;
            public byte name_num;
            public byte name_flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NCB
        {
            public byte ncb_command;
            public byte ncb_retcode;
            public byte ncb_lsn;
            public byte ncb_num;
            public IntPtr ncb_buffer;
            public ushort ncb_length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_callname;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_name;
            public byte ncb_rto;
            public byte ncb_sto;
            public IntPtr ncb_post;
            public byte ncb_lana_num;
            public byte ncb_cmd_cplt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] ncb_reserve;
            public IntPtr ncb_event;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LANA_ENUM
        {
            public byte length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.MAX_LANA)]
            public byte[] lana;
        }

        [StructLayout(LayoutKind.Auto)]
        private struct ASTAT
        {
            public ADAPTER_STATUS adapt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NUM_NAMEBUF)]
            public NAME_BUFFER[] NameBuff;
        }

        private class Win32API
        {
            [DllImport("NETAPI32.DLL")]
            public static extern char Netbios(ref NCB ncb);
        }

        // 取网卡 MAC 地址
        /// <summary>
        /// 取网卡 MAC 地址
        /// </summary>
        /// <returns></returns>
        internal static string GetMacAddress()
        {
            string addr = "";
            try
            {
                int cb;
                ASTAT adapter;
                NCB Ncb = new NCB();
                char uRetCode;
                LANA_ENUM lenum;
                Ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                cb = Marshal.SizeOf(typeof(LANA_ENUM));
                Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                Ncb.ncb_length = (ushort)cb;
                uRetCode = Win32API.Netbios(ref Ncb);
                lenum = (LANA_ENUM)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(LANA_ENUM));
                Marshal.FreeHGlobal(Ncb.ncb_buffer);
                if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                    return "";
                for (int i = 0; i < lenum.length; i++)
                {
                    Ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    uRetCode = Win32API.Netbios(ref Ncb);
                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";
                    Ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    Ncb.ncb_callname[0] = (byte)'*';
                    cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) + Marshal.SizeOf(typeof(NAME_BUFFER)) * (int)NCBCONST.NUM_NAMEBUF;
                    Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    Ncb.ncb_length = (ushort)cb;
                    uRetCode = Win32API.Netbios(ref Ncb);
                    adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                    Marshal.FreeHGlobal(Ncb.ncb_buffer);
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
            return addr.Replace(' ', '0');
        }

        #endregion
    }
}
