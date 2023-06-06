using System;
using Windows.ApplicationModel;

namespace FileRenamer.Helpers.Root
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

        /// <summary>
        /// 获取应用安装根目录
        /// </summary>
        public static string GetAppInstalledLocation()
        {
            return AppContext.BaseDirectory;
        }
    }
}
