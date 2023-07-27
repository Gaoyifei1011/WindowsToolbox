using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Services.Root;
using System;
using System.Threading.Tasks;

namespace FileRenamer.Services.Controls.Settings.Common
{
    /// <summary>
    /// 修改完成后自动打开文件夹设置服务
    /// </summary>
    public static class OpenFolderService
    {
        private static string SettingsKey { get; } = ConfigKey.OpenFolderKey;

        private static bool DefaultOpenFolderValue { get; } = false;

        public static bool OpenFolderValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的修改完成后自动打开文件夹设置值
        /// </summary>
        public static async Task InitializeOpenFolderValueAsync()
        {
            OpenFolderValue = await GetOpenFolderValueAsync();
        }

        /// <summary>
        /// 获取设置存储的修改完成后自动打开文件夹设置值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetOpenFolderValueAsync()
        {
            bool? openFolderValue = await ConfigService.ReadSettingAsync<bool?>(SettingsKey);

            if (!openFolderValue.HasValue)
            {
                return DefaultOpenFolderValue;
            }

            return Convert.ToBoolean(openFolderValue);
        }

        /// <summary>
        /// 自动打开文件夹设置值发生修改时修改设置存储的自动打开文件夹设置值
        /// </summary>
        public static async Task SetOpenFolderValueAsync(bool openFolderValue)
        {
            OpenFolderValue = openFolderValue;

            await ConfigService.SaveSettingAsync(SettingsKey, openFolderValue);
        }
    }
}
