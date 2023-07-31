using FileRenamer.Helpers.Root;
using FileRenamer.Models;
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

        private static ResourceContext DefaultResourceContext { get; set; }

        private static ResourceContext CurrentResourceContext { get; set; }

        private static ResourceContext UnpackagedResourceContext { get; set; }

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap;

        public static List<GroupOptionsModel> BackdropList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> ThemeList { get; } = new List<GroupOptionsModel>();

        static ResourceService()
        {
            if (RuntimeHelper.IsMSIX)
            {
                DefaultResourceContext = new ResourceContext();
                CurrentResourceContext = new ResourceContext();
            }
            else
            {
                UnpackagedResourceContext = ResourceContext.GetForViewIndependentUse();
            }
        }

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(GroupOptionsModel defaultAppLanguage, GroupOptionsModel currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage.SelectedValue;
            CurrentAppLanguage = currentAppLanguage.SelectedValue;

            if (RuntimeHelper.IsMSIX)
            {
                DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
                CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage;
            }
            else
            {
                UnpackagedResourceContext.QualifierValues["Language"] = CurrentAppLanguage;
            }

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
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeDefault"),
                SelectedValue = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeLight"),
                SelectedValue = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeDark"),
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
                DisplayMember = GetLocalized("Settings/BackdropDefault"),
                SelectedValue = "Default"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropMica"),
                SelectedValue = "Mica"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropMicaAlt"),
                SelectedValue = "MicaAlt"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropAcrylic"),
                SelectedValue = "Acrylic"
            });
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (IsInitialized)
            {
                if (RuntimeHelper.IsMSIX)
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
                    try
                    {
                        return ResourceMap.GetValue(resource, UnpackagedResourceContext).ValueAsString;
                    }
                    catch (NullReferenceException)
                    {
                        try
                        {
                            UnpackagedResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
                            string result = ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                            UnpackagedResourceContext.QualifierValues["Language"] = CurrentAppLanguage;
                            return result;
                        }
                        catch (NullReferenceException)
                        {
                            UnpackagedResourceContext.QualifierValues["Language"] = CurrentAppLanguage;
                            return resource;
                        }
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
