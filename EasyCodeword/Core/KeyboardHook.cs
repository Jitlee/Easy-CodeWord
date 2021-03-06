﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using EasyCodeword.Utilities;
using System.Security.Permissions;

namespace EasyCodeword.Core
{
    public class KeyboardHook
    {

        static int hKeyboardHook = 0;
        KeyboardProc KeyboardHookProcedure;

        private Func<bool> _callback;

        /// <summary>
        /// 钩子函数,需要引用空间(using System.Reflection;)
        /// 线程钩子监听键盘消息设为2,全局钩子监听键盘消息设为13
        /// 线程钩子监听鼠标消息设为7,全局钩子监听鼠标消息设为14
        /// </summary>

        public const int WH_KEYBOARD = 13;
        public const int WH_MOUSE_LL = 14;

        //private FileStream MyFs;

        //各种键位的ASC码
        private const byte LLKHF_ALTDOWN = 0x20;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_ESCAPE = 0x1B;
        private const byte VK_F4 = 0x73;
        private const byte VK_LCONTROL = 0xA2;
        private const byte VK_NUMLOCK = 0x90;
        private const byte VK_RCONTROL = 0xA3;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_TAB = 0x09;
        //public const int WH_KEYBOARD = 13;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE = 7;
        //private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        //private static int hKeyboardHook = 0;


        //在这里你可以自己定义要拦截的键。
        private int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyboardMSG m = (KeyboardMSG)Marshal.PtrToStructure(lParam, typeof(KeyboardMSG));
            if (
           ((int)m.vkCode == 91) || ((int)m.vkCode == 92) ||
                //两个组合键
           ((m.vkCode == VK_TAB) && ((m.flags & LLKHF_ALTDOWN) != 0)) ||

           ((m.vkCode == VK_ESCAPE) && ((m.flags & LLKHF_ALTDOWN) != 0)) ||
           ((m.vkCode == VK_F4) && ((m.flags & LLKHF_ALTDOWN) != 0)) ||
                //用于三个组合键
           (m.vkCode == VK_ESCAPE) && ((NativeMethods.GetKeyState(VK_LCONTROL) & 0x8000) != 0) ||
           ((int)m.vkCode >= 65 && (int)m.vkCode <= 90 && ((NativeMethods.GetKeyState(0x12) & 0x8000) != 0)) ||
           ((int)m.vkCode >= 65 && (int)m.vkCode <= 90 && ((NativeMethods.GetKeyState(0x11) & 0x8000) != 0)) ||
           (m.vkCode == VK_ESCAPE) && ((NativeMethods.GetKeyState(VK_RCONTROL) & 0x8000) != 0)
           )
            {
                if (null == _callback || (null != _callback && _callback()))
                {
                    return 1;
                }
                else
                {
                    KeyMaskStop();
                }
            }
            return NativeMethods.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        // 安装钩子
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        internal void KeyMaskStart(Func<bool> callback)
        {
            _callback = callback;
            if (hKeyboardHook == 0)
            {
                // 创建HookProc实例
                KeyboardHookProcedure = new KeyboardProc(KeyboardHookProc);

                // 设置线程钩子
                hKeyboardHook = NativeMethods.SetWindowsHookEx(WH_KEYBOARD, KeyboardHookProcedure,
                    NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                // 如果设置钩子失败
                if (hKeyboardHook == 0)
                {
                    KeyMaskStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
                ////用二进制流的方法打开任务管理器。而且不关闭流.这样任务管理器就打开不了
                //MyFs = new FileStream(Environment.ExpandEnvironmentVariables("%windir%\\system32\\taskmgr.exe"),
                //FileMode.Open);
                //byte[] MyByte = new byte[(int)MyFs.Length];
                //MyFs.Write(MyByte, 0, (int)MyFs.Length);
            }
        }

        // 卸载钩子
        public void KeyMaskStop()
        {
            _callback = null;
            bool retKeyboard = true;
            if (hKeyboardHook != 0)
            {
                retKeyboard = NativeMethods.UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            if (!(retKeyboard))
            {
                throw new Exception("UnhookWindowsHookEx  failed.");
            }
        }
    }
}
