using Microsoft.Win32;
using System;
using System.Diagnostics.Tracing;
using WindowsTools.Services.Root;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 注册表读取辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 读取注册表指定项的内容
        /// </summary>
        public static T ReadRegistryValue<T>(string rootkey, string key)
        {
            T value = default;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(rootkey);

                if (registryKey is not null)
                {
                    object getValue = registryKey.GetValue(key);

                    if (getValue is not null)
                    {
                        // 读取布尔值
                        if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                        {
                            value = (T)(object)Convert.ToBoolean(getValue);
                        }
                        else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                        {
                            value = (T)(object)Convert.ToInt32(getValue);
                        }
                        else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                        {
                            value = (T)(object)Convert.ToUInt32(getValue);
                        }
                        else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                        {
                            value = (T)(object)Convert.ToInt64(getValue);
                        }
                        else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                        {
                            value = (T)(object)Convert.ToUInt64(getValue);
                        }
                        else if (typeof(T) == typeof(string))
                        {
                            value = (T)(object)Convert.ToString(getValue);
                        }
                        else if (typeof(T) == typeof(byte[]) || typeof(T) == typeof(string[]))
                        {
                            value = (T)getValue;
                        }
                        else
                        {
                            value = (T)getValue;
                        }
                    }

                    registryKey.Close();
                    registryKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Read Registry rootkey {0} and key {1} value failed", rootkey, key), e);
            }

            return value;
        }

        /// <summary>
        /// 保存注册表指定项的内容
        /// </summary>
        public static void SaveRegistryValue<T>(string rootkey, string key, T value, bool isExpandString = false)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(rootkey, true);

                if (registryKey is not null)
                {
                    // 存储 32 位整数类型或者布尔值
                    if (typeof(T) == typeof(bool) || typeof(T) == typeof(int) || typeof(T) == typeof(uint))
                    {
                        int saveValue = Convert.ToInt32(value);

                        if (saveValue >= 0)
                        {
                            registryKey.SetValue(key, Convert.ToInt32(value), RegistryValueKind.DWord);
                        }
                    }
                    // 存储 64 位整数类型
                    else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
                    {
                        registryKey.SetValue(key, Convert.ToInt32(value), RegistryValueKind.QWord);
                    }
                    // 存储字符串类型
                    else if (typeof(T) == typeof(string))
                    {
                        if (isExpandString)
                        {
                            registryKey.SetValue(key, value, RegistryValueKind.String);
                        }
                        else
                        {
                            registryKey.SetValue(key, value, RegistryValueKind.ExpandString);
                        }
                    }
                    // 存储二进制数据
                    else if (typeof(T) == typeof(byte[]))
                    {
                        registryKey.SetValue(key, value, RegistryValueKind.Binary);
                    }
                    // 存储字符串数组
                    else if (typeof(T) == typeof(string[]))
                    {
                        registryKey.SetValue(key, value, RegistryValueKind.MultiString);
                    }
                    // 其他类型
                    else
                    {
                        registryKey.SetValue(key, value);
                    }

                    registryKey.Close();
                    registryKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Save Registry rootkey {0} and key {1} value failed", rootkey, key), e);
            }
        }
    }
}
