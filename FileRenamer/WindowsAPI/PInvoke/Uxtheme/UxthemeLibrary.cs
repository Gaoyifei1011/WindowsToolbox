using System;
using System.Runtime.InteropServices;

namespace FileRenamer.WindowsAPI.PInvoke.Uxtheme
{
    public static partial class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#135")]
        public static extern IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#136")]
        public static extern IntPtr FlushMenuThemes();
    }
}
