using PowerTools.Extensions.DataType.Constant;
using PowerTools.Services.Root;

namespace PowerTools.Services.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class FileShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.FileShellMenuKey;

        private static readonly bool defaultFileShellMenuValue = true;

        public static bool FileShellMenuValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的文件右键菜单显示值
        /// </summary>
        public static void InitializeFileShellMenu()
        {
            FileShellMenuValue = GetFileShellMenuValue();
        }

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetFileShellMenuValue()
        {
            bool? fileShellMenuValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!fileShellMenuValue.HasValue)
            {
                SetFileShellMenuValue(defaultFileShellMenuValue);
                return defaultFileShellMenuValue;
            }

            return fileShellMenuValue.Value;
        }

        /// <summary>
        /// 文件右键菜单显示值发生修改时修改设置存储的文件右键菜单显示值
        /// </summary>
        public static void SetFileShellMenuValue(bool fileShellMenuValue)
        {
            FileShellMenuValue = fileShellMenuValue;
            LocalSettingsService.SaveSetting(settingsKey, fileShellMenuValue);
        }
    }
}
