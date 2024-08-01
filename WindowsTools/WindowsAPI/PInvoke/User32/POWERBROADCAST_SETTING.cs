using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 随电源设置事件一起发送，并包含有关特定更改的数据。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POWERBROADCAST_SETTING
    {
        /// <summary>
        /// 指示发送此通知的电源设置。
        /// </summary>
        public Guid PowerSetting;

        /// <summary>
        /// Data 成员中数据的大小（以字节为单位）。
        /// </summary>
        public uint DataLength;

        /// <summary>
        /// 电源设置的新值。 此成员的类型和可能的值取决于 PowerSettng。
        /// </summary>
        public byte Data;
    }
}
