using WindowsToolsShellExtension.Extensions.DataType.Constant;
using WindowsToolsShellExtension.Services.Root;

namespace WindowsToolsShellExtension.Services.Controls.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static readonly string settingsKey = ConfigKey.LanguageKey;
        private static readonly string defaultAppLanguage = "en-us";

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        public static string GetLanguage()
        {
            string language = LocalSettingsService.ReadSetting<string>(settingsKey);
            return string.IsNullOrEmpty(language) ? defaultAppLanguage : language;
        }
    }
}
