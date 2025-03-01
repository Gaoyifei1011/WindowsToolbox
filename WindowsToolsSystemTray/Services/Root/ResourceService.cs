using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace WindowsToolsSystemTray.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        public static List<string> ThemeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeThemeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(Convert.ToString(ElementTheme.Default));
            ThemeList.Add(Convert.ToString(ElementTheme.Light));
            ThemeList.Add(Convert.ToString(ElementTheme.Dark));
        }
    }
}
