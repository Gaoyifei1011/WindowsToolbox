using System;
using System.Runtime.InteropServices;

namespace PowerTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 获取或设置一个值，该值指定当前线程上所有 DesktopWindowXamlSource 对象的背景是否透明。
    /// </summary>
    [ComImport, Guid("06636C29-5A17-458D-8EA2-2422D997A922"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IXamlSourceTransparency
    {
        /// <summary>
        /// 获取当前线程上所有 DesktopWindowXamlSource 对象的背景是否透明。
        /// </summary>
        /// <param name="isBackgroundTransparent">如果窗口背景透明，则为 true ;否则，为 false.</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetIsBackgroundTransparent([MarshalAs(UnmanagedType.Bool)] out bool isBackgroundTransparent);

        /// <summary>
        /// 设置当前线程上所有 DesktopWindowXamlSource 对象的背景是否透明。
        /// </summary>
        /// <param name="isBackgroundTransparent">如果窗口背景透明，则为 true ;否则，为 false.</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetIsBackgroundTransparent([MarshalAs(UnmanagedType.Bool)] bool isBackgroundTransparent);
    }
}
