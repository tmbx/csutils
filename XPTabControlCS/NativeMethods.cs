/* Copyright(c) 2008-2009 Vladimir Svyatsky
 * This code is provided "as is" without any warranty. You can modify it and use it in any applications
 * as long as you do not sell it as a component or a part of component library.
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VSControls
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", EntryPoint = "RealGetWindowClassW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder ClassName, uint ClassNameMax);

        #region API Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x, y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TCHITTESTINFO
        {
            public POINT pt;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd, hwndInsertAfter;
            public int x, y, cx, cy, flags;
        }
        #endregion

        #region Messages
        public const int TCM_HITTEST = 0x130d, WM_SETFONT = 0x0030, WM_THEMECHANGED = 0x031a,
            WM_DESTROY = 0x0002, WM_NCDESTROY = 0x0082, WM_WINDOWPOSCHANGING = 0x0046,
            WM_PARENTNOTIFY = 0x0210, WM_CREATE = 0x0001, WM_MOUSEMOVE = 0x0200, WM_LBUTTONDOWN = 0x0201;
        #endregion
    }
}