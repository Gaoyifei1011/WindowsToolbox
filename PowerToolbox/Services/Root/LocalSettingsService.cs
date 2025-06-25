using Microsoft.Win32;
using PowerToolbox.Helpers.Root;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static readonly string settingsKey = @"Software\PowerToolbox\Settings";

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            return RegistryHelper.ReadRegistryKey<T>(Registry.CurrentUser, settingsKey, key);
        }

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        public static void SaveSetting<T>(string key, T value)
        {
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, settingsKey, key, value);
        }
    }
}
