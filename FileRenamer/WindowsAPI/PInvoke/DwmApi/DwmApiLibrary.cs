using System;
using System.Runtime.InteropServices;

namespace FileRenamer.WindowsAPI.PInvoke.DwmApi
{
    public static class DwmApiLibrary
    {
        private const string DwmApi = "dwmapi.dll";

        [DllImport(DwmApi, CharSet = CharSet.Unicode, EntryPoint = "DwmSetWindowAttribute", SetLastError = false)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);
    }
}
