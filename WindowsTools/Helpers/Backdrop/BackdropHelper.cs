using System;
using Windows.Foundation;
using WindowsTools.WindowsAPI.ComTypes;
using WindowsTools.WindowsAPI.PInvoke.Combase;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools.Helpers.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        public static IPropertyValueStatics PropertyValueStatics { get; } = GetActivationFactory<IPropertyValueStatics>(typeof(PropertyValue).FullName, typeof(IPropertyValueStatics).GUID);

        /// <summary>
        /// 获取指定运行时类的激活工厂。
        /// </summary>
        public static T GetActivationFactory<T>(string activatableClassId, Guid iid)
        {
            if (!string.IsNullOrEmpty(activatableClassId))
            {
                CombaseLibrary.RoGetActivationFactory(activatableClassId, iid, out object comp);
                return (T)comp;
            }
            else
            {
                return default;
            }
        }
    }
}
