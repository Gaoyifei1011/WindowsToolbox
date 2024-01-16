using System;
using Windows.ApplicationModel;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        public static Version AppVersion { get; } = new Version(
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build,
            Package.Current.Id.Version.Revision
            );

        public static Version SystemVersion { get; } = Environment.OSVersion.Version;
    }
}
