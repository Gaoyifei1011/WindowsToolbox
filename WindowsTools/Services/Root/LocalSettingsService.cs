using WindowsTools.Helpers.Root;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static string settingsKey = @"Software\WindowsTools\Settings";

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            return RegistryHelper.ReadRegistryValue<T>(settingsKey, key);
        }

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        public static void SaveSetting<T>(string key, T value)
        {
            RegistryHelper.SaveRegistryValue(settingsKey, key, value);
        }
    }
}
