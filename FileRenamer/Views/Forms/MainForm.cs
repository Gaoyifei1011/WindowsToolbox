using FileRenamer.Helpers.Controls;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Services.Controls.Settings;
using FileRenamer.Services.Window;
using FileRenamer.Strings;
using FileRenamer.UI.Dialogs;
using FileRenamer.Views.Pages;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using FileRenamer.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Mile.Xaml;
using Mile.Xaml.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer.Views.Forms
{
    /// <summary>
    /// 应用主界面
    /// </summary>
    public class MainForm : Form
    {
        private int windowWidth = 1000;
        private int windowHeight = 700;
        private double WindowDPI;

        private IntPtr UWPCoreHandle;
        private IContainer components = new Container();
        private WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        private UISettings UISettings = new UISettings();

        public AppWindow AppWindow { get; }

        public MainPage MainPage { get; } = new MainPage();

        public MainForm()
        {
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            Controls.Add(MileXamlHost);
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            WindowDPI = ((double)DeviceDpi) / 96;
            MinimumSize = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            Size = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.AppDisplayName;

            if (RuntimeHelper.IsElevated)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
            }

            Microsoft.UI.WindowId windowId = new Microsoft.UI.WindowId();
            windowId.Value = (ulong)Handle;
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            SetTitleBarColor(MainPage.ActualTheme);

            IntPtr inputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, typeof(InputNonClientPointerSource).Name, null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                int style = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)(style & ~(int)WindowStyle.WS_SYSMENU));

                newInputNonClientPointerSourceWndProc = new WNDPROC(InputNonClientPointerSourceWndProc);
                oldInputNonClientPointerSourceWndProc = SetWindowLongAuto(inputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
            }

            UWPCoreHandle = InteropExtensions.GetInterop(Windows.UI.Xaml.Window.Current.CoreWindow).WindowHandle;
            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            UISettings.ColorValuesChanged += OnColorValuesChanged;
            MainPage.ActualThemeChanged += OnActualThemeChanged;

            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = MainPage;
        }

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 设备的每英寸像素 (PPI) 显示更改修改后触发的事件
        /// </summary>
        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);
            WindowDPI = ((double)args.DeviceDpiNew) / 96;
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);
            MainPage.ActualThemeChanged -= OnActualThemeChanged;
            UISettings.ColorValuesChanged -= OnColorValuesChanged;
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
        }

        /// <summary>
        /// 窗体移动时关闭浮出窗口
        /// </summary>
        protected override void OnMove(EventArgs args)
        {
            base.OnMove(args);
            if (MainPage.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(MainPage.XamlRoot);
                foreach (Popup popup in PopupRoot)
                {
                    // 关闭浮出控件
                    if (popup.Child as FlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭菜单浮出控件
                    if (popup.Child as MenuFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭日期选择器浮出控件
                    if (popup.Child as DatePickerFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭时间选择器浮出控件
                    if (popup.Child as TimePickerFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
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
            MainPage.IsWindowMaximized = WindowState == FormWindowState.Maximized;

            if (MainPage.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(MainPage.XamlRoot);
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

            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            BeginInvoke(SetAppTheme);
        }

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[0].Value))
            {
                if (sender.ActualTheme is ElementTheme.Light)
                {
                    MainPage.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    MainPage.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 20, 20, 20));
                }
            }

            SetTitleBarColor(sender.ActualTheme);
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
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
        /// 处理 Windows 消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case (int)WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (MainPage.TitlebarMenuFlyout.IsOpen)
                        {
                            MainPage.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 窗口接收其他数据消息
                case (int)WindowMessage.WM_COPYDATA:
                    {
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(m.LParam);

                        if (copyDataStruct.lpData is "AppIsRunning")
                        {
                            BeginInvoke(async () =>
                            {
                                await ContentDialogHelper.ShowAsync(new AppRunningDialog(), MainPage);
                            });
                        }
                        else if (copyDataStruct.lpData.Contains("FileName"))
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(FileNamePage))
                            {
                                NavigationService.NavigateTo(typeof(FileNamePage));
                            }

                            if (copyDataStruct.lpData.Contains("ShellContextMenu"))
                            {
                                FileNamePage fileNamePage = NavigationService.NavigationFrame.Content as FileNamePage;

                                if (fileNamePage is not null)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            if (File.Exists(Program.TempFilePath))
                                            {
                                                string tempFileText = File.ReadAllText(Program.TempFilePath);
                                                File.Delete(Program.TempFilePath);

                                                string[] fileNamePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();

                                                foreach (string fileNamePath in fileNamePathList)
                                                {
                                                    string fileName = IOHelper.FilterInvalidPathChars(fileNamePath);

                                                    BeginInvoke(() =>
                                                    {
                                                        fileNamePage.FileNameDataList.Add(new OldAndNewNameModel()
                                                        {
                                                            OriginalFileName = Path.GetFileName(fileName),
                                                            OriginalFilePath = fileName
                                                        });
                                                    });
                                                }
                                            }
                                        }
                                        catch (Exception) { }
                                    });
                                }
                            }
                        }
                        else if (copyDataStruct.lpData.Contains("ExtensionName"))
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(ExtensionNamePage))
                            {
                                NavigationService.NavigateTo(typeof(ExtensionNamePage));
                            }

                            if (copyDataStruct.lpData.Contains("ShellContextMenu"))
                            {
                                ExtensionNamePage extensionNamePage = NavigationService.NavigationFrame.Content as ExtensionNamePage;

                                if (extensionNamePage is not null)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            if (File.Exists(Program.TempFilePath))
                                            {
                                                string tempFileText = File.ReadAllText(Program.TempFilePath);
                                                File.Delete(Program.TempFilePath);

                                                string[] extensionNamePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                                foreach (string extensionNamePath in extensionNamePathList)
                                                {
                                                    string extensionName = IOHelper.FilterInvalidPathChars(extensionNamePath);

                                                    if (!IOHelper.IsDir(extensionName))
                                                    {
                                                        BeginInvoke(() =>
                                                        {
                                                            extensionNamePage.ExtensionNameDataList.Add(new OldAndNewNameModel()
                                                            {
                                                                OriginalFileName = Path.GetFileName(extensionName),
                                                                OriginalFilePath = extensionName
                                                            });
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception) { }
                                    });
                                }
                            }
                        }
                        else if (copyDataStruct.lpData.Contains("UpperAndLowerCase"))
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(UpperAndLowerCase))
                            {
                                NavigationService.NavigateTo(typeof(UpperAndLowerCasePage));
                            }

                            if (copyDataStruct.lpData.Contains("ShellContextMenu"))
                            {
                                UpperAndLowerCasePage upperAndLowerCasePage = NavigationService.NavigationFrame.Content as UpperAndLowerCasePage;

                                if (upperAndLowerCasePage is not null)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            if (File.Exists(Program.TempFilePath))
                                            {
                                                string tempFileText = File.ReadAllText(Program.TempFilePath);
                                                File.Delete(Program.TempFilePath);

                                                string[] upperAndLowerCasePathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                                foreach (string upperAndLowerCasePath in upperAndLowerCasePathList)
                                                {
                                                    string upperAndLowerCase = IOHelper.FilterInvalidPathChars(upperAndLowerCasePath);

                                                    BeginInvoke(() =>
                                                    {
                                                        upperAndLowerCasePage.UpperAndLowerCaseDataList.Add(new OldAndNewNameModel()
                                                        {
                                                            OriginalFileName = Path.GetFileName(upperAndLowerCase),
                                                            OriginalFilePath = upperAndLowerCase
                                                        });
                                                    });
                                                }
                                            }
                                        }
                                        catch (Exception) { }
                                    });
                                }
                            }
                        }
                        else if (copyDataStruct.lpData.Contains("FileProperties"))
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(FilePropertiesPage))
                            {
                                NavigationService.NavigateTo(typeof(FilePropertiesPage));
                            }

                            if (copyDataStruct.lpData.Contains("ShellContextMenu"))
                            {
                                FilePropertiesPage filePropertiesPage = NavigationService.NavigationFrame.Content as FilePropertiesPage;

                                if (filePropertiesPage is not null)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            if (File.Exists(Program.TempFilePath))
                                            {
                                                string tempFileText = File.ReadAllText(Program.TempFilePath);
                                                File.Delete(Program.TempFilePath);

                                                string[] filePropertiesPathList = tempFileText.Split('\n').Where(item => !string.IsNullOrEmpty(item)).ToArray();

                                                foreach (string filePropertiesPath in filePropertiesPathList)
                                                {
                                                    string fileProperties = IOHelper.FilterInvalidPathChars(filePropertiesPath);

                                                    BeginInvoke(() =>
                                                    {
                                                        filePropertiesPage.FilePropertiesDataList.Add(new OldAndNewPropertiesModel()
                                                        {
                                                            FileName = Path.GetFileName(fileProperties),
                                                            FilePath = fileProperties
                                                        });
                                                    });
                                                }
                                            }
                                        }
                                        catch (Exception) { }
                                    });
                                }
                            }
                        }
                        break;
                    };
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
                            options.Position = new Windows.Foundation.Point(0, 0);
                            options.ShowMode = FlyoutShowMode.Standard;
                            MainPage.TitlebarMenuFlyout.ShowAt(null, options);
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
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (MainPage.TitlebarMenuFlyout.IsOpen)
                        {
                            MainPage.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (MainPage is not null && MainPage.XamlRoot is not null)
                        {
                            Point ms = MousePosition;
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                                new Windows.Foundation.Point((ms.X - Location.X - 8) / WindowDPI, (ms.Y - Location.Y) / WindowDPI) :
                                new Windows.Foundation.Point(ms.X - Location.X - 8, ms.Y - Location.Y);
                            MainPage.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return IntPtr.Zero;
                    }
            }
            return User32Library.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor(ElementTheme theme)
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
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 255, 255, 255);
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
                if (Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
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
                if (MainPage.ActualTheme is ElementTheme.Light)
                {
                    MainPage.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    MainPage.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 20, 20, 20));
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[1].Value))
            {
                int micaBackdrop = 2;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));
                MainPage.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[2].Value))
            {
                int micaAltBackdrop = 4;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaAltBackdrop, Marshal.SizeOf(typeof(int)));
                MainPage.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[3].Value))
            {
                int acrylicBackdrop = 3;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref acrylicBackdrop, Marshal.SizeOf(typeof(int)));
                MainPage.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
