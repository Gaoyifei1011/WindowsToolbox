using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using WindowsToolsShellExtension.Extensions.Registry;
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
        public static T ReadRegistryKey<T>(string rootKey, string key)
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
                                value = typeof(T) == typeof(bool) || typeof(T) == typeof(bool?) ? (T)(object)BitConverter.ToBoolean(data, 0) : (T)(object)BitConverter.ToUInt32(data, 0);
                            }
                            // 字符串序列
                            else if (kind is RegistryValueKind.REG_MULTI_SZ)
                            {
                                List<string> stringList = [];
                                string packed = Encoding.Unicode.GetString(data, 0, length);
                                int start = 0;
                                int end = packed.IndexOf('\0', start);
                                while (end > start)
                                {
                                    stringList.Add(packed[start..end]);
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
                _ = Advapi32Library.RegCloseKey(hKey);
                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// 枚举并递归当前注册表项的所有子项
        /// </summary>
        public static RegistryEnumKeyItem EnumSubKey(string rootKey)
        {
            RegistryEnumKeyItem registryEnumKeyItem = new();

            try
            {
                UIntPtr hKey = UIntPtr.Zero;

                if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, rootKey, 0, RegistryAccessRights.KEY_READ, ref hKey) is 0)
                {
                    // 添加当前项信息
                    registryEnumKeyItem.RootKey = rootKey;

                    // 获取当前项的所有子项列表
                    List<string> subKeyList = [];
                    Span<char> name = stackalloc char[256];

                    int result;
                    int nameLength = name.Length;

                    while ((result = Advapi32Library.RegEnumKeyEx(hKey, subKeyList.Count, ref MemoryMarshal.GetReference(name), ref nameLength, null, null, null, null)) != 259)
                    {
                        if (result is 0)
                        {
                            subKeyList.Add(new string(name[..nameLength]));
                            nameLength = name.Length;
                        }
                        else if (result is 2) // ERROR_FILE_NOT_FOUND
                        {
                            continue;
                        }
                        else if (result is 5) // ERROR_ACCESS_DENIED
                        {
                            continue;
                        }
                    }

                    foreach (string subKey in subKeyList)
                    {
                        registryEnumKeyItem.SubRegistryKeyList.Add(EnumSubKey(Path.Combine(rootKey, subKey)));
                    }
                }

                return registryEnumKeyItem;
            }
            catch (Exception)
            {
                return registryEnumKeyItem;
            }
        }
    }
}
