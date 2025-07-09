using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Dnsapi
{
    /// <summary>
    /// Dnsapi.dll 函数库
    /// </summary>
    public static class DnsapiLibrary
    {
        private const string Dnsapi = "dnsapi.dll";

        [DllImport(Dnsapi, CharSet = CharSet.Unicode, EntryPoint = "DnsFlushResolverCache", PreserveSig = true, SetLastError = false)]
        public static extern bool DnsFlushResolverCache();
    }
}
