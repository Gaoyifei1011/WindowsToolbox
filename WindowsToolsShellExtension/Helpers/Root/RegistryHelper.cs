using GetStoreApp.WindowsAPI.PInvoke.Advapi32;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsToolsShellExtension.WindowsAPI.PInvoke.Advapi32;

namespace WindowsToolsShellExtension.Helpers.Root
{
    /// <summary>
    /// 注册表读取辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 读取注册表指定项的内容
        /// </summary>
        public static T ReadRegistryValue<T>(string rootKey, string key)
        {
            T value = default;
            try
            {
                UIntPtr hKey = UIntPtr.Zero;

                if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, rootKey, 0, RegistryAccessRights.KEY_READ, ref hKey) is 0)
                {
                    int length = 0;

                    // 读取数据大小
                    if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out _, null, ref length) is 0)
                    {
                        byte[] data = new byte[length];

                        // 根据数据大小读取数据内容
                        if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out RegistryValueKind kind, data, ref length) is 0)
                        {
                            // 字符串类型
                            if (kind is RegistryValueKind.REG_SZ)
                            {
                                value = (T)(object)Encoding.Unicode.GetString(data, 0, length);
                            }
                            // 字符串类型
                            else if (kind is RegistryValueKind.REG_EXPAND_SZ)
                            {
                                value = (T)(object)Environment.ExpandEnvironmentVariables(Encoding.Unicode.GetString(data, 0, length));
                            }
                            // 二进制数据类型
                            else if (kind is RegistryValueKind.REG_BINARY)
                            {
                                value = (T)(object)data;
                            }
                            // 32 位数字或布尔值
                            else if (kind is RegistryValueKind.REG_DWORD || kind is RegistryValueKind.REG_DWORD_LITTLE_ENDIAN || kind is RegistryValueKind.REG_DWORD_BIG_ENDIAN)
                            {
                                if (!BitConverter.IsLittleEndian)
                                {
                                    Array.Reverse(data);
                                }

                                // 布尔值以字符串整数类型值存储
                                if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                                {
                                    value = (T)(object)BitConverter.ToBoolean(data, 0);
                                }
                                else
                                {
                                    value = (T)(object)BitConverter.ToUInt32(data, 0);
                                }
                            }
                            // 字符串序列
                            else if (kind is RegistryValueKind.REG_MULTI_SZ)
                            {
                                List<string> stringList = new List<string>();
                                string packed = Encoding.Unicode.GetString(data, 0, length);
                                int start = 0;
                                int end = packed.IndexOf('\0', start);
                                while (end > start)
                                {
                                    stringList.Add(packed.Substring(start, end - start));
                                    start = end + 1;
                                    end = packed.IndexOf('\0', start);
                                }
                                value = (T)(object)stringList.ToArray();
                            }
                            // 64 位数字
                            else if (kind is RegistryValueKind.REG_QWORD || kind is RegistryValueKind.REG_QWORD_LITTLE_ENDIAN)
                            {
                                if (!BitConverter.IsLittleEndian)
                                {
                                    Array.Reverse(data);
                                }
                                value = (T)(object)BitConverter.ToUInt64(data, 0);
                            }
                        }
                    }
                }
                Advapi32Library.RegCloseKey(hKey);
                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
