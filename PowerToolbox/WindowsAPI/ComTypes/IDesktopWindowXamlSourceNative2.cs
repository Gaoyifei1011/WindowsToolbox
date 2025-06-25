using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    [ComImport, Guid("e3dcd8c7-3057-4692-99c3-7b7720afda31"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopWindowXamlSourceNative2
    {
        /// <summary>
        /// 将当前 IDesktopWindowXamlSourceNative 实例附加到桌面应用中与窗口句柄关联的父 UI 元素。
        /// </summary>
        /// <param name="parentWnd">要在其中托管 WinRT XAML 控件的父 UI 元素的窗口句柄。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int AttachToWindow(IntPtr parentWnd);

        /// <summary>
        /// 获取与当前 IDesktopWindowXamlSourceNative 实例关联的父 UI 元素的窗口句柄。
        /// </summary>
        /// <param name="hwnd">在输出时，此参数包含与当前 IDesktopWindowXamlSourceNative 实例关联的父 UI 元素的窗口句柄。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetWindowHandle(out IntPtr hwnd);

        /// <summary>
        /// 使 WinRT XAML 框架能够处理托管 WinRT XAML 控件的 DesktopWindowXamlSource 对象的 Windows 消息。
        /// </summary>
        /// <param name="message">要处理的 Windows 消息。</param>
        /// <param name="result">如果消息已处理，则为 True;否则为 false。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int PreTranslateMessage(ref MSG message, out bool result);
    }
}
