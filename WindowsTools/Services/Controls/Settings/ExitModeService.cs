using System.Collections.Generic;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用程序退出方式设置服务
    /// </summary>
    public static class ExitModeService
    {
        private static readonly string settingsKey = ConfigKey.ExitModeKey;

        private static KeyValuePair<string, string> defaultExitMode;

        public static KeyValuePair<string, string> ExitMode { get; set; }

        public static List<KeyValuePair<string, string>> ExitModeList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的退出方式值
        /// </summary>
        public static void InitializeExitMode()
        {
            ExitModeList = ResourceService.ExitModeList;

            defaultExitMode = ExitModeList[0];

            ExitMode = GetExitMode();
        }

        /// <summary>
        /// 获取设置存储的退出方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetExitMode()
        {
            string exitMode = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(exitMode))
            {
                SetExitMode(defaultExitMode);
                return defaultExitMode;
            }

            KeyValuePair<string, string> selectedExitMode = ExitModeList.Find(item => item.Key.Equals(exitMode));

            return selectedExitMode.Key is null ? defaultExitMode : ExitModeList.Find(item => item.Key.Equals(exitMode));
        }

        /// <summary>
        /// 应用退出方式发生修改时修改设置存储的退出方式值
        /// </summary>
        public static void SetExitMode(KeyValuePair<string, string> exitMode)
        {
            ExitMode = exitMode;

            LocalSettingsService.SaveSetting(settingsKey, exitMode.Key);
        }
    }
}
