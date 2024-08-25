using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// Advapi32.dll 函数库
    /// </summary>
    public static class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// 通知调用方对指定注册表项的属性或内容的更改。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="watchSubtree">如果此参数为 TRUE，则函数将报告指定键及其子项中的更改。 如果参数为 FALSE，则函数仅报告指定键中的更改。</param>
        /// <param name="notifyFilter">一个值，该值指示应报告的更改。</param>
        /// <param name="hEvent">事件的句柄。 如果 fAsynchronous 参数为 TRUE，则函数将立即返回 ，并通过发出此事件信号来报告更改。 如果 fAsynchronous 为 FALSE，则忽略 hEvent 。</param>
        /// <param name="asynchronous">如果此参数为 TRUE，则函数将立即返回并通过向指定事件发出信号来报告更改。 如果此参数为 FALSE，则函数在发生更改之前不会返回 。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [DllImport(Advapi32, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "RegNotifyChangeKeyValue", SetLastError = false), PreserveSig]
        public static extern int RegNotifyChangeKeyValue(UIntPtr hKey, [MarshalAs(UnmanagedType.Bool)] bool watchSubtree, REG_NOTIFY_FILTER notifyFilter, IntPtr hEvent, [MarshalAs(UnmanagedType.Bool)] bool asynchronous);
    }
}
