using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Shell32
{
    public static class Shell32Library
    {
        public const string Shell32 = "shell32.dll";

        [DllImport(Shell32, CharSet = CharSet.Unicode, EntryPoint = "SHGetKnownFolderPath", SetLastError = true)]
        public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);
    }
}
