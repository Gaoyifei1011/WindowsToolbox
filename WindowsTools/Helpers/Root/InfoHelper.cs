using System;
using System.Reflection;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        public static Version AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        public static Version SystemVersion { get; } = Environment.OSVersion.Version;
    }
}
