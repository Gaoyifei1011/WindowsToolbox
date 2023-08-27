using FileRenamer.Models;
using FileRenamer.Strings;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace FileRenamer.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        public static List<GroupOptionsModel> BackdropList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> ThemeList { get; } = new List<GroupOptionsModel>();

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeBackdropList();
            InitializeThemeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.ThemeDefault,
                SelectedValue = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.ThemeLight,
                SelectedValue = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.ThemeDark,
                SelectedValue = Convert.ToString(ElementTheme.Dark)
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.BackdropDefault,
                SelectedValue = "Default"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.BackdropMica,
                SelectedValue = "Mica"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.BackdropMicaAlt,
                SelectedValue = "MicaAlt"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = Settings.BackdropAcrylic,
                SelectedValue = "Acrylic"
            });
        }
    }
}
