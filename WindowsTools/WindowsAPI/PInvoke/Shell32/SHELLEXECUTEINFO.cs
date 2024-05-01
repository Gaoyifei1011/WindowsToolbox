using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Shell32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpVerb;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpFile;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpParameters;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDirectory;

        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpClass;

        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }
}
