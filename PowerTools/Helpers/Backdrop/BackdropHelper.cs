using PowerTools.WindowsAPI.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerTools.Helpers.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        public static IPropertyValueStatics PropertyValueStatics { get; } = (IPropertyValueStatics)WindowsRuntimeMarshal.GetActivationFactory(typeof(PropertyValue));
    }
}
