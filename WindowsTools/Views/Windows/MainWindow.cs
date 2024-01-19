using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Mile.Xaml;
using Mile.Xaml.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Views.Pages;
using WindowsTools.WindowsAPI.PInvoke.DwmApi;
using WindowsTools.WindowsAPI.PInvoke.User32;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 应用主界面
    /// </summary>
    public class MainWindow : Form
    {
        private int windowWidth = 1024;
        private int windowHeight = 768;
        private double WindowDPI;

        private IntPtr UWPCoreHandle;
        private IntPtr InputNonClientPointerSourceHandle;
        private IContainer components = new Container();
        private WindowsXamlHost WindowsXamlHost = new WindowsXamlHost();

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        public AppWindow AppWindow { get; }

        public UIElement Content { get; set; } = new MainPage();

        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            Current = this;
            Controls.Add(WindowsXamlHost);
            WindowDPI = (double)DeviceDpi / 96;
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            MinimumSize = new System.Drawing.Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            Size = new System.Drawing.Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            StartPosition = FormStartPosition.CenterParent;
            Text = Strings.Window.AppTitle;
            WindowsXamlHost.AutoSize = true;
            WindowsXamlHost.Dock = DockStyle.Fill;
            WindowsXamlHost.Child = Content;

            if (RuntimeHelper.IsElevated)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
            }

            AppWindow = AppWindow.GetFromWindowId(new Microsoft.UI.WindowId() { Value = (ulong)Handle });
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            SetTitleBarColor((Content as FrameworkElement).ActualTheme);

            InputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, typeof(InputNonClientPointerSource).Name, null);

            if (InputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                int style = (int)GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)(style & ~(int)WindowStyle.WS_SYSMENU));

                newInputNonClientPointerSourceWndProc = new WNDPROC(InputNonClientPointerSourceWndProc);
                oldInputNonClientPointerSourceWndProc = SetWindowLongAuto(InputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
            }

            UWPCoreHandle = InteropExtensions.GetInterop(global::Windows.UI.Xaml.Window.Current.CoreWindow).WindowHandle;
            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
                long style = (long)GetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_STYLE);
                style &= ~(long)WindowStyle.WS_POPUP;
                SetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_STYLE, (nint)(style | (long)WindowStyle.WS_CHILDWINDOW | (long)WindowStyle.WS_VISIBLE));
                SetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_EXSTYLE, (IntPtr)((int)GetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_EXSTYLE) | (int)WindowStyleEx.WS_EX_TOOLWINDOW | (int)WindowStyleEx.WS_EX_TRANSPARENT));
            }
        }

        #region 第一部分：窗口类内置需要重载的事件

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && components is not null)
            {
                components.Dispose();
            }
        }

        /// <summary>
        /// 设备的每英寸像素 (PPI) 显示更改修改后触发的事件
        /// </summary>
        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);
            WindowDPI = (double)args.DeviceDpiNew / 96;

            if (InputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
            {
                User32Library.SetWindowPos(InputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * WindowDPI), 0, (int)((Width - 45) * WindowDPI), (int)(45 * WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);
            if (RuntimeHelper.IsElevated && Handle != IntPtr.Zero)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_RESET, in changeFilterStatus);
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            ThemeService.SetWindowTheme();
            BackdropService.SetAppBackdrop();
            TopMostService.SetAppTopMost();
            Margins FormMargin = new Margins();
            DwmApiLibrary.DwmExtendFrameIntoClientArea(Handle, ref FormMargin);
            Invalidate();

            if (InputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
            {
                User32Library.SetWindowPos(InputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * WindowDPI), 0, (int)((Width - 45) * WindowDPI), (int)(45 * WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 窗体移动时关闭浮出窗口
        /// </summary>
        protected override void OnMove(EventArgs args)
        {
            base.OnMove(args);
            if ((Content as FrameworkElement).XamlRoot is not null)
            {
                IReadOnlyList<Popup> popupRootList = VisualTreeHelper.GetOpenPopupsForXamlRoot((Content as FrameworkElement).XamlRoot);
                foreach (Popup popupRoot in popupRootList)
                {
                    // 关闭浮出控件
                    if (popupRoot.Child as FlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                        break;
                    }

                    // 关闭菜单浮出控件
                    if (popupRoot.Child as MenuFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                        break;
                    }

                    // 关闭日期选择器浮出控件
                    if (popupRoot.Child as DatePickerFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                        break;
                    }

                    // 关闭时间选择器浮出控件
                    if (popupRoot.Child as TimePickerFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 窗口大小改变时发生的事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs args)
        {
            base.OnSizeChanged(args);
            (Content as MainPage).IsWindowMaximized = WindowState is FormWindowState.Maximized;

            if (Content.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot);
                foreach (Popup popup in PopupRoot)
                {
                    // 关闭内容对话框
                    if (popup.Child as ContentDialog is not null)
                    {
                        (popup.Child as ContentDialog).Hide();
                        break;
                    }
                }
            }

            if (InputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
            {
                User32Library.SetWindowPos(InputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * WindowDPI), 0, (int)((Width - 45) * WindowDPI), (int)(45 * WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        #endregion 第一部分：窗口类内置需要重载的事件

        #region 第二部分：窗口过程

        /// <summary>
        /// 处理 Windows 消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        SetAppTheme();
                        break;
                    }
                // 窗口处理视觉样式对于此窗口是否处于活动状态
                case (int)WindowMessage.WM_NCACTIVATE:
                    {
                        if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                        {
                            m.WParam = (IntPtr)1;
                        }
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case (int)WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(m.WParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU || sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new global::Windows.Foundation.Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            return;
                        }
                        break;
                    }
                // 任务栏窗口右键点击后的消息
                case (int)WindowMessage.WM_SYSMENU:
                    {
                        if (WindowState is FormWindowState.Minimized)
                        {
                            WindowState = FormWindowState.Normal;
                        }
                        break;
                    }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceWndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 处理自定义标题栏窗口最大化时，窗口顶部标题栏还可以修改窗口大小的问题
                case WindowMessage.WM_NCHITTEST:
                    {
                        if (WindowState is FormWindowState.Maximized)
                        {
                            if (lParam.ToInt32() >> 16 < 4)
                            {
                                return (IntPtr)2;  // HTCAPTION （在标题栏中）
                            }
                        }
                        break;
                    }
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if ((Content as MainPage).TitlebarMenuFlyout.IsOpen)
                        {
                            (Content as MainPage).TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            System.Drawing.Point clientPoint = PointToClient(MousePosition);

                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                                new global::Windows.Foundation.Point(clientPoint.X / WindowDPI, clientPoint.Y / WindowDPI) :
                                new global::Windows.Foundation.Point(clientPoint.X, clientPoint.Y);
                            (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return IntPtr.Zero;
                    }
            }
            return User32Library.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
        }

        #endregion 第二部分：窗口过程

        #region 第三部分：窗口属性设置

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private IntPtr GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.GetWindowLongPtr(hWnd, nIndex);
            }
            else
            {
                return User32Library.GetWindowLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// 更改指定窗口的窗口属性
        /// </summary>
        private IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        public void SetTitleBarColor(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 设置应用的主题色
        /// </summary>
        public void SetAppTheme()
        {
            if (ThemeService.AppTheme.Value.Equals(ThemeService.ThemeList[0].Value))
            {
                if (global::Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    int useLightMode = 0;
                    DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
                }
                else
                {
                    int useDarkMode = 1;
                    DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
                }
            }
            else if (ThemeService.AppTheme.Value.Equals(ThemeService.ThemeList[1].Value))
            {
                int useLightMode = 0;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
            }
            else if (ThemeService.AppTheme.Value.Equals(ThemeService.ThemeList[2].Value))
            {
                int useDarkMode = 1;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
            }
        }

        /// <summary>
        /// 添加窗口背景色
        /// </summary>
        public void SetWindowBackdrop()
        {
            if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[0].Value))
            {
                int noBackdrop = 0;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref noBackdrop, Marshal.SizeOf(typeof(int)));
                if ((Content as MainPage).ActualTheme is ElementTheme.Light)
                {
                    (Content as MainPage).Background = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    (Content as MainPage).Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[1].Value))
            {
                int micaBackdrop = 2;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));
                (Content as MainPage).Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[2].Value))
            {
                int micaAltBackdrop = 4;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaAltBackdrop, Marshal.SizeOf(typeof(int)));
                (Content as MainPage).Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[3].Value))
            {
                int acrylicBackdrop = 3;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref acrylicBackdrop, Marshal.SizeOf(typeof(int)));
                (Content as MainPage).Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        #endregion 第三部分：窗口属性设置
    }
}
