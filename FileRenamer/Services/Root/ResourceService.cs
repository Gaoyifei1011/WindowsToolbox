using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Properties;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;

namespace FileRenamer.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool IsInitialized { get; set; } = false;

        private static string DefaultAppLanguage { get; set; }

        private static string CurrentAppLanguage { get; set; }

        private static ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private static ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap;

        public static List<BackdropModel> BackdropList { get; } = new List<BackdropModel>();

        public static List<ThemeModel> ThemeList { get; } = new List<ThemeModel>();

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage.InternalName;
            CurrentAppLanguage = currentAppLanguage.InternalName;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage;

            IsInitialized = true;
        }

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
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeDefault"),
                InternalName = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeLight"),
                InternalName = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeDark"),
                InternalName = Convert.ToString(ElementTheme.Dark)
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropDefault"),
                InternalName = "Default"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropMica"),
                InternalName = "Mica"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropMicaAlt"),
                InternalName = "MicaAlt"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropAcrylic"),
                InternalName = "Acrylic"
            });
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (IsInitialized)
            {
                try
                {
                    return ResourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                    }
                    catch (NullReferenceException)
                    {
                        return resource;
                    }
                }
            }
            else
            {
                throw new ApplicationException(Resources.ResourcesInitializeFailed);
            }
        }
    }
}
