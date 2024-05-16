using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;
using WindowsTools.WindowsAPI.PInvoke.Shell32;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static string settingsKey = ConfigKey.DownloadFolderKey;
        private static string doEngineModeKey = ConfigKey.DoEngineModeKey;

        private static string defaultDownloadFolder;
        private static DictionaryEntry defaultDoEngineMode;

        public static string DownloadFolder { get; private set; }

        public static DictionaryEntry DoEngineMode { get; private set; }

        public static List<DictionaryEntry> DoEngineModeList { get; } = ResourceService.DoEngineModeList;

        /// <summary>
        /// 应用在初始化前获取设置存储的下载目录设置值，并创建默认下载目录
        /// </summary>
        public static void InitializeDownload()
        {
            Shell32Library.SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, IntPtr.Zero, out string downloadFolder);
            defaultDownloadFolder = downloadFolder;
            defaultDoEngineMode = DoEngineModeList[0];
            DownloadFolder = GetFolder();
            DoEngineMode = GetDoEngineMode();
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
        /// 获取设置存储的下载引擎方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetDoEngineMode()
        {
            object doEngineMode = LocalSettingsService.ReadSetting<object>(doEngineModeKey);

            if (doEngineMode is null)
            {
                SetDoEngineMode(defaultDoEngineMode);
                return DoEngineModeList.Find(item => item.Value.Equals(defaultDoEngineMode.Value));
            }

            DictionaryEntry selectedDoEngine = DoEngineModeList.Find(item => item.Value.Equals(doEngineMode));

            return selectedDoEngine.Key is null ? defaultDoEngineMode : selectedDoEngine;
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(string downloadFolder)
        {
            DownloadFolder = downloadFolder;

            LocalSettingsService.SaveSetting(settingsKey, downloadFolder);
        }

        /// <summary>
        /// 应用下载引擎发生修改时修改设置存储的下载引擎方式值
        /// </summary>
        public static void SetDoEngineMode(DictionaryEntry doEngineMode)
        {
            DoEngineMode = doEngineMode;

            LocalSettingsService.SaveSetting(doEngineModeKey, doEngineMode.Value);
        }
    }
}
