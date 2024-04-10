using Microsoft.Win32;
using System;
using System.Diagnostics.Tracing;
using Windows.Storage;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static string windowsToolsKey = @"Software\WindowsTools";

        private static bool isInitialized = false;

        /// <summary>
        /// 初始化注册表
        /// </summary>
        public static void Initialize()
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(windowsToolsKey);
                registryKey.SetValue("ApplicationDataPath", ApplicationData.Current.LocalCacheFolder.Path, RegistryValueKind.String);
                registryKey.Close();
                registryKey.Dispose();
                isInitialized = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Registry settings saving initialize failed", e);
            }
        }

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            try
            {
                T readResult = default;
                if (isInitialized)
                {
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(windowsToolsKey);
                    if (registryKey is not null)
                    {
                        object value = registryKey.GetValue(key);

                        if (value is not null)
                        {
                            if (typeof(T) == typeof(bool?))
                            {
                                readResult = (T)(object)Convert.ToBoolean(value);
                            }
                            else if (typeof(T) == typeof(int?))
                            {
                                readResult = (T)(object)Convert.ToInt32(value);
                            }
                            else if (typeof(T) == typeof(string))
                            {
                                readResult = (T)(object)Convert.ToString(value);
                            }
                            else
                            {
                                readResult = (T)value;
                            }
                        }

                        registryKey.Close();
                        registryKey.Dispose();
                    }
                }
                return readResult;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Registry settings read value {0} failed", key), e);
                return default;
            }
        }

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        public static void SaveSetting<T>(string key, T value)
        {
            try
            {
                if (isInitialized)
                {
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(windowsToolsKey, true);
                    if (registryKey is not null)
                    {
                        if (value.GetType() == typeof(bool) || value.GetType() == typeof(int))
                        {
                            registryKey.SetValue(key, value, RegistryValueKind.DWord);
                        }
                        else if (value.GetType() == typeof(string))
                        {
                            registryKey.SetValue(key, value, RegistryValueKind.String);
                        }
                        registryKey.Close();
                        registryKey.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Registry settings save key {0} value {1} failed", key, value), e);
            }
        }
    }
}
