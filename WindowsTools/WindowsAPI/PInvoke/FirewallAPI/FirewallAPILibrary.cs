using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.FirewallAPI
{
    public static class FirewallAPILibrary
    {
        public const string FirewallAPI = "FirewallAPI.dll";

        /// <summary>
        /// 枚举已在系统中创建的所有应用容器。
        /// </summary>
        /// <param name="Flags">可以设置为 NETISO_FLAG_FORCE_COMPUTE_BINARIES ，以确保在返回应用容器之前计算所有二进制文件。 如果调用方需要有关应用容器二进制文件的最新完整信息，则应设置此标志。 如果未设置此标志，则返回的数据可能已过时或不完整。</param>
        /// <param name="pdwCntPublicACs">应用容器数。</param>
        /// <param name="ppPublicACs">应用容器结构元素的列表。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。如果内存不可用，将返回ERROR_OUTOFMEMORY。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationEnumAppContainers", SetLastError = false)]
        internal static extern uint NetworkIsolationEnumAppContainers(NETISO_FLAG Flags, out uint pdwCntPublicACs, out IntPtr ppPublicACs);

        /// <summary>
        /// 用于释放分配给一个或多个应用容器的内存资源
        /// </summary>
        /// <param name="pPublicAppCs">要释放的应用容器内存资源。</param>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationFreeAppContainers", SetLastError = false)]
        internal static extern void NetworkIsolationFreeAppContainers(IntPtr pPublicAppCs);

        /// <summary>
        /// 用于检索有关一个或多个应用容器的配置信息。
        /// </summary>
        /// <param name="pdwNumPublicAppCs">appContainerSids 成员中的应用容器数。</param>
        /// <param name="appContainerSids">安全标识符 (允许发送环回流量的应用容器) SID。 用于调试目的。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationGetAppContainerConfig", SetLastError = false)]
        internal static extern uint NetworkIsolationGetAppContainerConfig(out uint pdwNumPublicAppCs, out IntPtr appContainerSids);

        /// <summary>
        /// 用于设置一个或多个应用容器的配置。
        /// </summary>
        /// <param name="dwNumPublicAppCs">appContainerSids 成员中的应用容器数。</param>
        /// <param name="appContainerSids">安全标识符 (允许发送环回流量的应用容器) SID。 用于调试目的。</param>
        /// <returns>如果成功，则返回ERROR_SUCCESS，否则返回错误值。</returns>
        [DllImport(FirewallAPI, CharSet = CharSet.Unicode, EntryPoint = "NetworkIsolationSetAppContainerConfig", SetLastError = false)]
        internal static extern uint NetworkIsolationSetAppContainerConfig(int dwNumPublicAppCs, [MarshalAs(UnmanagedType.LPArray)] SID_AND_ATTRIBUTES[] appContainerSids);
    }
}
