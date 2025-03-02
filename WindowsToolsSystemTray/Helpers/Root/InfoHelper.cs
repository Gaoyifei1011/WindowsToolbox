using System;

namespace WindowsToolsSystemTray.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        public static Version SystemVersion { get; } = Environment.OSVersion.Version;
    }
}
