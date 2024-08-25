namespace WindowsToolsShellExtension.Helpers.Root
{
    /// <summary>
    /// 系统主题辅助类
    /// </summary>
    public static class ThemeHelper
    {
        public static bool AppsUseLightTheme { get; } = false;

        static ThemeHelper()
        {
            bool? appsUseLightTheme = RegistryHelper.ReadRegistryKey<bool?>(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme");
            AppsUseLightTheme = appsUseLightTheme.HasValue && appsUseLightTheme.Value;
        }
    }
}
