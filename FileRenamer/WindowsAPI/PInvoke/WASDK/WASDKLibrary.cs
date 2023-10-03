using System;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

namespace FileRenamer.WindowsAPI.PInvoke.WASDK
{
    /// <summary>
    /// FileRenamerWASDK.dll 函数库
    /// </summary>
    public static class WASDKLibrary
    {
        private const string WASDK = "FileRenamerWASDK.dll";

        /// <summary>
        /// 初始化 AppWindow 窗口辅助类
        /// </summary>
        /// <param name="hwnd">要输入的窗口句柄</param>
        [DllImport(WASDK, EntryPoint = "InitializeAppWindow", SetLastError = false)]
        public static extern void InitializeAppWindow(IntPtr hwnd);

        /// <summary>
        /// 扩展标题栏，替换默认窗口标题栏
        /// </summary>
        /// <param name="value">是否要使用自定义的窗口标题栏</param>
        /// <returns>true 如果应隐藏默认标题栏，否则为 false。</returns>
        [DllImport(WASDK, EntryPoint = "ExtendsContentToTitleBar", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExtendsContentToTitleBar(bool value);

        /// <summary>
        /// 设置标题栏按钮的主题颜色
        /// </summary>
        /// <param name="theme">对应的主题值</param>
        [DllImport(WASDK, EntryPoint = "SetWindowTitleBarColor", SetLastError = false)]
        public static extern void SetWindowTitleBarColor(ElementTheme theme);

        /// <summary>
        /// 卸载 AppWindow 窗口辅助类
        /// </summary>
        [DllImport(WASDK, EntryPoint = "UnInitializeAppWindow", SetLastError = false)]
        public static extern void UnInitializeAppWindow();
    }
}
