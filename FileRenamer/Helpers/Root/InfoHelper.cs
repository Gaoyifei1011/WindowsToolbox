using System;
using System.Diagnostics;
using Windows.ApplicationModel;

namespace FileRenamer.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        public static Version AppVersion { get; }

        public static Version SystemVersion { get; } = Environment.OSVersion.Version;

        static InfoHelper()
        {
            if (RuntimeHelper.IsMSIX)
            {
                AppVersion = new Version(
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision
                    );
            }
            else
            {
                FileVersionInfo WindowsUIFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}{1}", AppContext.BaseDirectory, "FileRenamer.exe"));
                AppVersion = new Version(
                    WindowsUIFile.ProductMajorPart,
                    WindowsUIFile.ProductMinorPart,
                    WindowsUIFile.ProductBuildPart,
                    WindowsUIFile.ProductPrivatePart
                    );
            }
        }
    }
}
