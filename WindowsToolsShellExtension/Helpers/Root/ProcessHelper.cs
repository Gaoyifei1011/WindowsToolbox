using System;
using System.Runtime.InteropServices;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.Kernel32;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.User32;

namespace WindowsToolsShellExtension.Helpers.Root
{
    /// <summary>
    /// 进程辅助类
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 创建进程
        /// </summary>
        public static unsafe void StartProcess(string processName, string arguments, out int processid)
        {
            Kernel32Library.GetStartupInfo(out STARTUPINFO startupInfo);
            startupInfo.lpReserved = IntPtr.Zero;
            startupInfo.lpDesktop = IntPtr.Zero;
            startupInfo.lpTitle = IntPtr.Zero;
            startupInfo.dwX = 0;
            startupInfo.dwY = 0;
            startupInfo.dwXSize = 0;
            startupInfo.dwYSize = 0;
            startupInfo.dwXCountChars = 500;
            startupInfo.dwYCountChars = 500;
            startupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
            startupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
            startupInfo.cbReserved2 = 0;
            startupInfo.lpReserved2 = IntPtr.Zero;
            startupInfo.cb = sizeof(STARTUPINFO);

            bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", processName, arguments), IntPtr.Zero, IntPtr.Zero, false, CREATE_PROCESS_FLAGS.None, IntPtr.Zero, null, ref startupInfo, out PROCESS_INFORMATION processInformation);

            if (createResult)
            {
                if (processInformation.hProcess != IntPtr.Zero)
                {
                    Kernel32Library.CloseHandle(processInformation.hProcess);
                }

                if (processInformation.hThread != IntPtr.Zero)
                {
                    Kernel32Library.CloseHandle(processInformation.hThread);
                }

                processid = processInformation.dwProcessId;
            }
            else
            {
                processid = 0;
            }
        }
    }
}
