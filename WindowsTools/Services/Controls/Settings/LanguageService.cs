using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Windows.Globalization;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static readonly string resourceFileName = string.Format("{0}.resources.dll", Assembly.GetExecutingAssembly().GetName().Name);
        private static readonly string settingsKey = ConfigKey.LanguageKey;
        private static DictionaryEntry defaultAppLanguage;

        public static DictionaryEntry AppLanguage { get; private set; }

        public static RightToLeft RightToLeft { get; private set; }

        private static readonly List<string> AppLanguagesList = [];

        public static List<DictionaryEntry> LanguageList { get; } = [];

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            try
            {
                string[] resourceFolder = Directory.GetFiles(AppContext.BaseDirectory, resourceFileName, SearchOption.AllDirectories);

                foreach (string file in resourceFolder)
                {
                    AppLanguagesList.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                }
                AppLanguagesList.Sort();
            }
            catch
            {
                AppLanguagesList.Clear();
                AppLanguagesList.Add("en-us");
            }

            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);

                LanguageList.Add(new DictionaryEntry(culture.NativeName, culture.Name));
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(CultureInfo currentCulture, out DictionaryEntry language)
        {
            foreach (DictionaryEntry languageItem in LanguageList)
            {
                if (languageItem.Value.ToString().Equals(currentCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    language = languageItem;
                    return true;
                }
            }

            language = new();
            return false;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static void InitializeLanguage()
        {
            InitializeLanguageList();

            defaultAppLanguage = LanguageList.Find(item => item.Value.ToString().Equals("en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = GetLanguage();
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetLanguage()
        {
            string language = LocalSettingsService.ReadSetting<string>(settingsKey);

            // 当前系统语言和当前系统语言的父区域性的语言
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture.Parent.Parent;
            CultureInfo currentParentCultureInfo = CultureInfo.CurrentCulture.Parent.Parent.Parent;
            bool existResult = false;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                existResult = IsExistsInLanguageList(currentCultureInfo, out DictionaryEntry currentLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (existResult)
                {
                    SetLanguage(currentLanguage);
                    RightToLeft = currentCultureInfo.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                    return currentLanguage;
                }
                else
                {
                    existResult = IsExistsInLanguageList(currentParentCultureInfo, out DictionaryEntry currentParentLanguage);

                    // 如果存在，设置存储值和应用初次设置的语言为当前系统语言的父区域性的语言
                    if (existResult)
                    {
                        SetLanguage(currentParentLanguage);
                        RightToLeft = currentParentCultureInfo.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                        return currentParentLanguage;
                    }

                    // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                    else
                    {
                        SetLanguage(defaultAppLanguage);
                        CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(defaultAppLanguage.Value.ToString());
                        RightToLeft = defaultCultureInfo.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                        return defaultAppLanguage;
                    }
                }
            }

            CultureInfo savedCultureInfo = CultureInfo.GetCultureInfo(language.ToString());
            RightToLeft = savedCultureInfo.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            return LanguageList.Find(item => language.Equals(item.Value.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(DictionaryEntry language)
        {
            AppLanguage = language;
            LocalSettingsService.SaveSetting(settingsKey, language.Value);
            ApplicationLanguages.PrimaryLanguageOverride = language.Value.ToString();
        }
    }
}
