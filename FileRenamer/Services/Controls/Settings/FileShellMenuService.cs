using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Services.Root;

namespace FileRenamer.Services.Controls.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class FileShellMenuService
    {
        private static string SettingsKey { get; } = ConfigKey.FileShellMenuKey;

        private static bool DefaultFileShellMenuValue = true;

        public static bool FileShellMenuValue { get; set; }

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
            bool? fileShellMenuValue = ConfigService.ReadSetting<bool?>(SettingsKey);

            if (!fileShellMenuValue.HasValue)
            {
                SetFileShellMenu(DefaultFileShellMenuValue);
                return DefaultFileShellMenuValue;
            }

            return fileShellMenuValue.Value;
        }

        /// <summary>
        /// 文件右键菜单显示值发生修改时修改设置存储的文件右键菜单显示值
        /// </summary>
        public static void SetFileShellMenu(bool fileShellMenuValue)
        {
            FileShellMenuValue = fileShellMenuValue;

            ConfigService.SaveSetting(SettingsKey, fileShellMenuValue);
        }
    }
}
