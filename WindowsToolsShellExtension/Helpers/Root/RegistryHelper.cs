using GetStoreApp.WindowsAPI.PInvoke.Advapi32;
using System;
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
        /// 读取注册表根键值和键值对应的内容
        /// </summary>
        public static T GetValue<T>(string rootKey, string key)
        {
            try
            {
                nuint hKey = nuint.Zero;
                T readResult = default;

                if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, rootKey, 0, RegistryAccessRights.KEY_READ, ref hKey) is 0)
                {
                    uint dataSize = 0;

                    // 读取数据大小
                    if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out _, null, ref dataSize) is 0)
                    {
                        byte[] data = new byte[dataSize];

                        // 根据数据大小读取数据内容
                        if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out _, data, ref dataSize) is 0)
                        {
                            if (typeof(T) == typeof(bool))
                            {
                                readResult = (T)(object)Convert.ToBoolean(data[0]);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                readResult = (T)(object)Convert.ToInt32(data[0]);
                            }
                            else if (typeof(T) == typeof(string))
                            {
                                readResult = (T)(object)Encoding.Unicode.GetString(data);
                            }
                            else
                            {
                                readResult = (T)(object)data;
                            }
                        }
                    }
                }
                Advapi32Library.RegCloseKey(hKey);
                return readResult;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
