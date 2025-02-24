using System;
using WindowsTools.WindowsAPI.PInvoke.Combase;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// WinRT 辅助类
    /// </summary>
    public static class WinRTHelper
    {
        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        public static object GetActivationFactory(string activatableClassId, Guid iid)
        {
            if (string.IsNullOrEmpty(activatableClassId) || iid.Equals(Guid.Empty))
            {
                return null;
            }
            else
            {
                if (CombaseLibrary.RoGetActivationFactory(activatableClassId, iid, out object comp) is 0)
                {
                    return comp;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
