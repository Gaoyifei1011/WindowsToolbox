using PowerToolbox.Extensions.DataType.Constant;
using PowerToolbox.Services.Root;
using System;
using System.ComponentModel;

namespace PowerToolbox.Services.Settings
{
    /// <summary>
    /// 自动切换主题服务
    /// </summary>
    public static class AutoThemeSwitchService
    {
        private static readonly string autoThemeSwitchEnableKey = ConfigKey.AutoThemeSwitchEnableKey;
        private static readonly string autoSwitchSystemThemeKey = ConfigKey.AutoSwitchSystemThemeKey;
        private static readonly string autoSwitchAppThemeKey = ConfigKey.AutoSwitchAppThemeKey;
        private static readonly string isShowColorInDarkThemeKey = ConfigKey.IsShowColorInDarkThemeKey;
        private static readonly string systemThemeLightTimeKey = ConfigKey.SystemThemeLightTimeKey;
        private static readonly string systemThemeDarkTimeKey = ConfigKey.SystemThemeDarkTimeKey;
        private static readonly string appThemeLightTimeKey = ConfigKey.AppThemeLightTimeKey;
        private static readonly string appThemeDarkTimeKey = ConfigKey.AppThemeDarkTimeKey;

        public static bool DefaultAutoThemeSwitchEnableValue { get; } = false;

        public static bool DefaultAutoSwitchSystemThemeValue { get; } = false;

        public static bool DefaultAutoSwitchAppThemeValue { get; } = false;

        public static bool DefaultIsShowColorInDarkThemeValue { get; } = false;

        public static TimeSpan DefaultSystemThemeLightTime { get; } = new(7, 0, 0);

        public static TimeSpan DefaultSystemThemeDarkTime { get; } = new(19, 0, 0);

        public static TimeSpan DefaultAppThemeLightTime { get; } = new(7, 0, 0);

        public static TimeSpan DefaultAppThemeDarkTime { get; } = new(19, 0, 0);

        private static bool _autoThemeSwitchEnableValue;

        public static bool AutoThemeSwitchEnableValue
        {
            get { return _autoThemeSwitchEnableValue; }

            private set
            {
                if (!Equals(_autoThemeSwitchEnableValue, value))
                {
                    _autoThemeSwitchEnableValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AutoThemeSwitchEnableValue)));
                }
            }
        }

        private static bool _autoSwitchSystemThemeValue;

        public static bool AutoSwitchSystemThemeValue
        {
            get { return _autoSwitchSystemThemeValue; }

            private set
            {
                if (!Equals(_autoSwitchSystemThemeValue, value))
                {
                    _autoSwitchSystemThemeValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AutoSwitchSystemThemeValue)));
                }
            }
        }

        private static bool _autoSwitchAppThemeValue;

        public static bool AutoSwitchAppThemeValue
        {
            get { return _autoSwitchAppThemeValue; }

            private set
            {
                if (!Equals(_autoSwitchAppThemeValue, value))
                {
                    _autoSwitchAppThemeValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AutoSwitchAppThemeValue)));
                }
            }
        }

        private static bool _isShowColorInDarkThemeValue;

        public static bool IsShowColorInDarkThemeValue
        {
            get { return _isShowColorInDarkThemeValue; }

            private set
            {
                if (!Equals(_isShowColorInDarkThemeValue, value))
                {
                    _isShowColorInDarkThemeValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsShowColorInDarkThemeValue)));
                }
            }
        }

        private static TimeSpan _systemThemeLightTime;

        public static TimeSpan SystemThemeLightTime
        {
            get { return _systemThemeLightTime; }

            private set
            {
                if (!Equals(_systemThemeLightTime, value))
                {
                    _systemThemeLightTime = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(SystemThemeLightTime)));
                }
            }
        }

        private static TimeSpan _systemThemeDarkTime;

        public static TimeSpan SystemThemeDarkTime
        {
            get { return _systemThemeDarkTime; }

            private set
            {
                if (!Equals(_systemThemeDarkTime, value))
                {
                    _systemThemeDarkTime = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(SystemThemeDarkTime)));
                }
            }
        }

        private static TimeSpan _appThemeLightTime;

        public static TimeSpan AppThemeLightTime
        {
            get { return _appThemeLightTime; }

            private set
            {
                if (!Equals(_appThemeLightTime, value))
                {
                    _appThemeLightTime = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppThemeLightTime)));
                }
            }
        }

        private static TimeSpan _appThemeDarkTime;

        public static TimeSpan AppThemeDarkTime
        {
            get { return _appThemeDarkTime; }

            private set
            {
                if (!Equals(_appThemeDarkTime, value))
                {
                    _appThemeDarkTime = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppThemeDarkTime)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的自动切换主题所有选项值
        /// </summary>
        public static void InitializeAutoThemeSwitch()
        {
            AutoThemeSwitchEnableValue = GetAutoThemeSwitchEnableValue();
            AutoSwitchSystemThemeValue = GetAutoSwitchSystemThemeValue();
            AutoSwitchAppThemeValue = GetAutoSwitchAppThemeValue();
            IsShowColorInDarkThemeValue = GetIsShowColorInDarkThemeValue();
            SystemThemeLightTime = GetSystemThemeLightTime();
            SystemThemeDarkTime = GetSystemThemeDarkTime();
            AppThemeLightTime = GetAppThemeLightTime();
            AppThemeDarkTime = GetAppThemeDarkTime();
        }

        /// <summary>
        /// 获取设置存储的自动切换主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoThemeSwitchEnableValue()
        {
            bool? autoThemeSwitchEnableValue = LocalSettingsService.ReadSetting<bool?>(autoThemeSwitchEnableKey);

            if (!autoThemeSwitchEnableValue.HasValue)
            {
                SetAutoThemeSwitchEnableValue(DefaultAutoThemeSwitchEnableValue);
                return DefaultAutoThemeSwitchEnableValue;
            }

            return autoThemeSwitchEnableValue.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换系统主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoSwitchSystemThemeValue()
        {
            bool? autoSwitchSystemThemeValue = LocalSettingsService.ReadSetting<bool?>(autoSwitchSystemThemeKey);

            if (!autoSwitchSystemThemeValue.HasValue)
            {
                SetAutoSwitchSystemThemeValue(DefaultAutoSwitchSystemThemeValue);
                return DefaultAutoSwitchSystemThemeValue;
            }

            return autoSwitchSystemThemeValue.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换应用主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoSwitchAppThemeValue()
        {
            bool? autoSwitchAppThemeValue = LocalSettingsService.ReadSetting<bool?>(autoSwitchAppThemeKey);

            if (!autoSwitchAppThemeValue.HasValue)
            {
                SetAutoSwitchAppThemeValue(DefaultAutoSwitchAppThemeValue);
                return DefaultAutoSwitchAppThemeValue;
            }

            return autoSwitchAppThemeValue.Value;
        }

        /// <summary>
        /// 获取设置存储的切换系统深色主题时显示主题色值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetIsShowColorInDarkThemeValue()
        {
            bool? isShowColorInDarkTheme = LocalSettingsService.ReadSetting<bool?>(isShowColorInDarkThemeKey);

            if (!isShowColorInDarkTheme.HasValue)
            {
                SetIsShowColorInDarkThemeValue(DefaultIsShowColorInDarkThemeValue);
                return DefaultIsShowColorInDarkThemeValue;
            }

            return isShowColorInDarkTheme.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换系统浅色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetSystemThemeLightTime()
        {
            string systemThemeLightTimeString = LocalSettingsService.ReadSetting<string>(systemThemeLightTimeKey);

            if (string.IsNullOrEmpty(systemThemeLightTimeString))
            {
                SetSystemThemeLightTime(DefaultSystemThemeLightTime);
                return DefaultSystemThemeLightTime;
            }
            else
            {
                if (TimeSpan.TryParse(systemThemeLightTimeString, out TimeSpan systemThemeLightTheme))
                {
                    SetSystemThemeLightTime(systemThemeLightTheme);
                    return systemThemeLightTheme;
                }
                else
                {
                    SetSystemThemeLightTime(DefaultSystemThemeLightTime);
                    return DefaultSystemThemeLightTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换系统深色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetSystemThemeDarkTime()
        {
            string systemThemeDarkTimeString = LocalSettingsService.ReadSetting<string>(systemThemeDarkTimeKey);

            if (string.IsNullOrEmpty(systemThemeDarkTimeString))
            {
                SetSystemThemeDarkTime(DefaultSystemThemeDarkTime);
                return DefaultSystemThemeDarkTime;
            }
            else
            {
                if (TimeSpan.TryParse(systemThemeDarkTimeString, out TimeSpan systemThemeLightTheme))
                {
                    SetSystemThemeDarkTime(systemThemeLightTheme);
                    return systemThemeLightTheme;
                }
                else
                {
                    SetSystemThemeDarkTime(DefaultSystemThemeDarkTime);
                    return DefaultSystemThemeDarkTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换应用浅色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetAppThemeLightTime()
        {
            string appThemeLightTimeString = LocalSettingsService.ReadSetting<string>(appThemeLightTimeKey);

            if (string.IsNullOrEmpty(appThemeLightTimeString))
            {
                SetAppThemeLightTime(DefaultAppThemeLightTime);
                return DefaultAppThemeLightTime;
            }
            else
            {
                if (TimeSpan.TryParse(appThemeLightTimeString, out TimeSpan appThemeLightTheme))
                {
                    SetAppThemeLightTime(appThemeLightTheme);
                    return appThemeLightTheme;
                }
                else
                {
                    SetAppThemeLightTime(DefaultAppThemeLightTime);
                    return DefaultAppThemeLightTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换应用深色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetAppThemeDarkTime()
        {
            string appThemeDarkTimeString = LocalSettingsService.ReadSetting<string>(appThemeDarkTimeKey);

            if (string.IsNullOrEmpty(appThemeDarkTimeString))
            {
                SetAppThemeDarkTime(DefaultAppThemeDarkTime);
                return DefaultAppThemeDarkTime;
            }
            else
            {
                if (TimeSpan.TryParse(appThemeDarkTimeString, out TimeSpan appThemeLightTheme))
                {
                    SetAppThemeDarkTime(appThemeLightTheme);
                    return appThemeLightTheme;
                }
                else
                {
                    SetAppThemeDarkTime(DefaultAppThemeDarkTime);
                    return DefaultAppThemeDarkTime;
                }
            }
        }

        /// <summary>
        /// 自动切换主题启用值发生修改时修改设置存储的自动切换主题启用值
        /// </summary>
        public static void SetAutoThemeSwitchEnableValue(bool autoThemeSwitchEnableValue)
        {
            AutoThemeSwitchEnableValue = autoThemeSwitchEnableValue;
            LocalSettingsService.SaveSetting(autoThemeSwitchEnableKey, autoThemeSwitchEnableValue);
        }

        /// <summary>
        /// 自动切换系统主题启用值发生修改时修改设置存储的自动切换系统主题启用值
        /// </summary>
        public static void SetAutoSwitchSystemThemeValue(bool autoSwitchSystemThemeValue)
        {
            AutoSwitchSystemThemeValue = autoSwitchSystemThemeValue;
            LocalSettingsService.SaveSetting(autoSwitchSystemThemeKey, autoSwitchSystemThemeValue);
        }

        /// <summary>
        /// 自动切换应用主题启用值发生修改时修改设置存储的自动切换系统主题启用值
        /// </summary>
        public static void SetAutoSwitchAppThemeValue(bool autoSwitchAppThemeValue)
        {
            AutoSwitchAppThemeValue = autoSwitchAppThemeValue;
            LocalSettingsService.SaveSetting(autoSwitchAppThemeKey, autoSwitchAppThemeValue);
        }

        /// <summary>
        /// 切换系统深色主题时显示主题色值发生修改时修改设置存储的切换系统深色主题时显示主题色值
        /// </summary>
        public static void SetIsShowColorInDarkThemeValue(bool isShowColorInDarkThemeValue)
        {
            IsShowColorInDarkThemeValue = isShowColorInDarkThemeValue;
            LocalSettingsService.SaveSetting(isShowColorInDarkThemeKey, isShowColorInDarkThemeValue);
        }

        /// <summary>
        /// 自动切换系统浅色主题时间值发生修改时修改设置存储的自动切换系统浅色主题时间值
        /// </summary>
        public static void SetSystemThemeLightTime(TimeSpan systemThemeLightTime)
        {
            SystemThemeLightTime = systemThemeLightTime;
            LocalSettingsService.SaveSetting(systemThemeLightTimeKey, systemThemeLightTime.ToString());
        }

        /// <summary>
        /// 自动切换系统深色主题时间值发生修改时修改设置存储的自动切换系统深色主题时间值
        /// </summary>
        public static void SetSystemThemeDarkTime(TimeSpan systemThemeDarkTime)
        {
            SystemThemeDarkTime = systemThemeDarkTime;
            LocalSettingsService.SaveSetting(systemThemeDarkTimeKey, systemThemeDarkTime.ToString());
        }

        /// <summary>
        /// 自动切换系统浅色主题时间值发生修改时修改设置存储的自动切换系统浅色主题时间值
        /// </summary>
        public static void SetAppThemeLightTime(TimeSpan appThemeLightTime)
        {
            AppThemeLightTime = appThemeLightTime;
            LocalSettingsService.SaveSetting(appThemeLightTimeKey, appThemeLightTime.ToString());
        }

        /// <summary>
        /// 自动切换系统深色主题时间值发生修改时修改设置存储的自动切换系统深色主题时间值
        /// </summary>
        public static void SetAppThemeDarkTime(TimeSpan appThemeDarkTime)
        {
            AppThemeDarkTime = appThemeDarkTime;
            LocalSettingsService.SaveSetting(appThemeDarkTimeKey, appThemeDarkTime.ToString());
        }
    }
}
