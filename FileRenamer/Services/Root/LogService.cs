using System;
using System.IO;
using System.Text;
using Windows.Globalization.DateTimeFormatting;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Root
{
    public enum LogType
    {
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
    }

    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static bool IsInitialized { get; set; } = false;

        private static StorageFolder LogFolder { get; set; }

        private static DateTimeFormatter dateTimeFormatter = new DateTimeFormatter("month day year hour minute second");

        private static DateTimeFormatter fileNameFormatter = new DateTimeFormatter("{year.full}_{month.integer}_{day.integer}");

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static async void Initialize()
        {
            LogFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Log", CreationCollisionOption.OpenIfExists);

            IsInitialized = true;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LogType logType, string logContent)
        {
            if (IsInitialized)
            {
                File.AppendAllText(
                    Path.Combine(LogFolder.Path, string.Format("FileRenamer_{0}.log", fileNameFormatter.Format(DateTime.Now))),
                    string.Format("{0}\t{1}\t{2}\n", dateTimeFormatter.Format(DateTime.Now), Convert.ToString(logType), logContent),
                    Encoding.UTF8
                    );
            }
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static async void OpenLogFolderAsync()
        {
            if (IsInitialized)
            {
                if (Directory.Exists(LogFolder.Path))
                {
                    await Launcher.LaunchFolderAsync(LogFolder);
                }
            }
        }
    }
}
