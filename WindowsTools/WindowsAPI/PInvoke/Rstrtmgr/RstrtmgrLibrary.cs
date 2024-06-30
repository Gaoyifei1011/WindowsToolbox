using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Rstrtmgr
{
    public static class RstrtmgrLibrary
    {
        public const string Rstrtmgr = "rstrtmgr.dll";

        /// <summary>
        /// 将资源注册到重启管理器会话。 重启管理器使用向会话注册的资源列表来确定必须关闭和重启哪些应用程序和服务。 可以通过文件名、服务短名称或描述正在运行的应用程序 RM_UNIQUE_PROCESS 结构来标识资源。 RmRegisterResources 函数可由主安装程序或辅助安装程序使用。
        /// </summary>
        /// <param name="pSessionHandle">现有重启管理器会话的句柄。</param>
        /// <param name="nFiles">正在注册的文件数。</param>
        /// <param name="rgsFilenames">完整文件名路径的 以 null 结尾的字符串数组。 如果 nFiles 为 0，此参数可以为 NULL。</param>
        /// <param name="nApplications">正在注册的进程数。</param>
        /// <param name="rgApplications">RM_UNIQUE_PROCESS 结构的数组。 如果 nApplications 为 0，此参数可以为 NULL。</param>
        /// <param name="nServices">要注册的服务数。</param>
        /// <param name="rgsServiceNames">以 null 结尾的服务短名称字符串的数组。 如果 nServices 为 0，此参数可以为 NULL。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode, EntryPoint = "RmRegisterResources", SetLastError = false)]
        internal static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

        /// <summary>
        /// 启动新的重启管理器会话。 每个用户会话最多可以同时在系统上打开 64 个重启管理器会话。 当此函数启动会话时，它将返回会话句柄和会话密钥，这些句柄和会话密钥可用于对重启管理器 API 的后续调用。
        /// </summary>
        /// <param name="pSessionHandle">指向重启管理器会话句柄的指针。 会话句柄可以在后续调用中传递给重启管理器 API。</param>
        /// <param name="dwSessionFlags">保留。 此参数应为 0。</param>
        /// <param name="strSessionKey">一个 以 null 结尾的字符串，其中包含新会话的会话密钥。 在调用 RmStartSession 函数之前，必须分配字符串。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode, EntryPoint = "RmStartSession", SetLastError = false)]
        internal static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        /// <summary>
        /// 结束重启管理器会话。 此函数应由之前通过调用 RmStartSession 函数启动会话的主安装程序调用。 RmEndSession 函数可由加入会话的辅助安装程序调用，辅助安装程序无需再注册更多资源。
        /// </summary>
        /// <param name="pSessionHandle">现有 Restart Manager 会话的句柄。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode, EntryPoint = "RmEndSession", SetLastError = false)]
        internal static extern int RmEndSession(uint pSessionHandle);

        /// <summary>
        /// 获取当前使用已注册到 Restart Manager 会话的资源的所有应用程序和服务的列表。
        /// </summary>
        /// <param name="dwSessionHandle">现有 Restart Manager 会话的句柄。</param>
        /// <param name="pnProcInfoNeeded">指向数组大小的指针，用于接收返回所有受影响应用程序和服务的信息所需的 RM_PROCESS_INFO 结构。</param>
        /// <param name="pnProcInfo">指向数组中 RM_PROCESS_INFO 结构总数和填充结构数的指针。</param>
        /// <param name="rgAffectedApps">一组RM_PROCESS_INFO结构，这些结构使用已注册到会话的资源列出应用程序和服务。</param>
        /// <param name="lpdwRebootReasons">指向位置的指针，该位置接收 RM_REBOOT_REASON 枚举的值，该枚举描述需要重启系统的原因。</param>
        /// <returns>这是收到的最新错误。 函数可以返回 Winerror.h 中定义的系统错误代码之一。</returns>
        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode, EntryPoint = "RmGetList", SetLastError = false)]
        internal static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In][Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);
    }
}
