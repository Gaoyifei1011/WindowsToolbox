using Windows.Foundation;
using WindowsTools.Helpers.Root;
using WindowsTools.WindowsAPI.ComTypes;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools.Helpers.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        public static IPropertyValueStatics PropertyValueStatics { get; } = (IPropertyValueStatics)WinRTHelper.GetActivationFactory(typeof(PropertyValue).FullName, typeof(IPropertyValueStatics).GUID);
    }
}
