﻿using System.Runtime.InteropServices;
using System.Text;

namespace WindowsTools.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        /// <summary>
        /// 获取当前进程的 应用程序用户模型 ID 。
        /// </summary>
        /// <param name="applicationUserModelIdLength">输入时， applicationUserModelId 缓冲区的大小（以宽字符为单位）。 成功时，使用的缓冲区大小，包括 null 终止符。</param>
        /// <param name="applicationUserModelId">指向接收应用程序用户模型 ID 的缓冲区的指针。</param>
        /// <returns>如果该函数成功，则返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentApplicationUserModelId", SetLastError = true)]
        internal static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, StringBuilder applicationUserModelId);

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">输入时， packageFullName 缓冲区的大小（以字符为单位）。 输出时，返回包全名的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFullName", SetLastError = true)]
        internal static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        /// <summary>
        /// 使应用程序能够通知系统它正在使用中，从而防止系统在应用程序运行时进入睡眠状态或关闭显示器。
        /// </summary>
        /// <param name="esFlags">线程的执行要求。</param>
        /// <returns>如果函数成功，则返回值为上一个线程执行状态。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "SetThreadExecutionState", SetLastError = true)]
        internal static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
