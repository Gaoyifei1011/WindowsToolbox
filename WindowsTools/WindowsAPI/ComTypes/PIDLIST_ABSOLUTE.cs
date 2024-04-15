using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 相对于名称空间根的完全限定ITEMIDLIST。它可能是多层次的。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PIDLIST_ABSOLUTE
    {
        /// <summary>
        /// 指向相对于名称空间根的完全限定ITEMIDLIST的指针。它可能是多层次的。
        /// </summary>
        public IntPtr Ptr;
    }
}
