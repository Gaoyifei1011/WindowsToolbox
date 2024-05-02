using System;
using System.Diagnostics.Tracing;
using System.IO;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DownloadService
    {
        private static string settingsKey = ConfigKey.DownloadFolderKey;

        private static string defaultDownloadFolder;

        public static string DownloadFolder { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载目录设置值，并创建默认下载目录
        /// </summary>
        public static void InitializeDownload()
        {
            Shell32Library.SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
            defaultDownloadFolder = downloadFolder;
            DownloadFolder = GetFolder();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static string GetFolder()
        {
            string folder = LocalSettingsService.ReadSetting<string>(settingsKey);

            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    SetFolder(defaultDownloadFolder);
                    return defaultDownloadFolder;
                }
                else
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    return folder;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Warning, "Get download saved folder failed.", e);
                SetFolder(defaultDownloadFolder);
                return defaultDownloadFolder;
            }
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(string downloadFolder)
        {
            DownloadFolder = downloadFolder;

            LocalSettingsService.SaveSetting(settingsKey, downloadFolder);
        }
    }
}
