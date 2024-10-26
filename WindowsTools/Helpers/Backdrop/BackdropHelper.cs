using System;
using System.Threading;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.UI.Composition.Desktop;
using Windows.UI.Xaml;
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
        public static Lazy<IPropertyValueStatics> PropertyValueStatics { get; } = new(() => GetActivationFactory<IPropertyValueStatics>(typeof(PropertyValue).FullName, typeof(IPropertyValueStatics).GUID));

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

        /// <summary>
        /// 初始化 DesktopWindowTarget（表示作为合成目标的窗口）
        /// </summary>
        public static DesktopWindowTarget InitializeDesktopWindowTarget(Form form, bool isTopMost)
        {
            if (form.Handle == IntPtr.Zero)
            {
                throw new NullReferenceException("窗口尚未初始化");
            }

            DesktopWindowTarget desktopWindowTarget = null;
            if (SynchronizationContext.Current is not null)
            {
                ICompositorDesktopInterop interop = Window.Current.Compositor as object as ICompositorDesktopInterop;
                interop.CreateDesktopWindowTarget(form.Handle, isTopMost, out desktopWindowTarget);
            }
            return desktopWindowTarget;
        }
    }
}
