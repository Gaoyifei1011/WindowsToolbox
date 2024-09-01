using Microsoft.UI.Windowing;
using Mile.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI;
using Windows.UI.Composition.Desktop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using WindowsTools.Helpers.Backdrop;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Strings;
using WindowsTools.UI.Backdrop;
using WindowsTools.Views.Pages;
using WindowsTools.WindowsAPI.PInvoke.Comctl32;
using WindowsTools.WindowsAPI.PInvoke.Shell32;
using WindowsTools.WindowsAPI.PInvoke.User32;

namespace WindowsTools.Views.Windows
{
    /// <summary>
    /// 应用主界面
    /// </summary>
    public class MainWindow : Form
    {
        private readonly IntPtr inputNonClientPointerSourceHandle;
        private readonly IContainer components = new Container();
        private readonly WindowsXamlHost windowsXamlHost = new();
        private readonly SUBCLASSPROC inputNonClientPointerSourceSubClassProc;

        private DesktopWindowTarget desktopWindowTarget;
        private SystemBackdrop systemBackdrop;

        private AppWindow AppWindow { get; }

        public UIElement Content { get; set; } = new MainPage();

        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Controls.Add(windowsXamlHost);
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            MinimumSize = new System.Drawing.Size(Convert.ToInt32(1024 * ((double)DeviceDpi) / 96), Convert.ToInt32(768 * ((double)DeviceDpi / 96)));
            Size = new System.Drawing.Size(Convert.ToInt32(1024 * ((double)DeviceDpi) / 96), Convert.ToInt32(768 * ((double)DeviceDpi / 96)));
            StartPosition = FormStartPosition.CenterScreen;
            Text = Strings.Window.AppTitle;
            windowsXamlHost.AutoSize = true;
            windowsXamlHost.Dock = DockStyle.Fill;
            windowsXamlHost.Child = Content;
            RightToLeft = LanguageService.RightToLeft;
            RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes;

            if (RuntimeHelper.IsElevated)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new()
                {
                    cbSize = Marshal.SizeOf<CHANGEFILTERSTRUCT>()
                };
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_DROPFILES, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYGLOBALDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
                Shell32Library.DragAcceptFiles(Handle, true);
            }

            AppWindow = AppWindow.GetFromWindowId(new Microsoft.UI.WindowId() { Value = (ulong)Handle });
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            SetTitleBarColor((Content as FrameworkElement).ActualTheme);

            inputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                inputNonClientPointerSourceSubClassProc = new SUBCLASSPROC(InputNonClientPointerSourceSubClassProc);
                Comctl32Library.SetWindowSubclass(Handle, inputNonClientPointerSourceSubClassProc, 0, IntPtr.Zero);
            }

            AlwaysShowBackdropService.PropertyChanged += OnServicePropertyChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;

            List<MenuItem> menuItemList =
            [
                new MenuItem() { Text = SystemTray.ShowOrHideWindow ,Tag = nameof(SystemTray.ShowOrHideWindow) },
                new MenuItem() { Text = "-" },
                new MenuItem() { Text = SystemTray.Exit,Tag = nameof(SystemTray.Exit) }
            ];

            SystemTrayService.InitializeSystemTray(Strings.Window.AppTitle, Process.GetCurrentProcess().MainModule.FileName, menuItemList);
            SystemTrayService.MenuItemClick += OnMenuItemClick;
            SystemTrayService.MouseDoubleClick += OnSystemTrayDoubleClick;
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

            if (inputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
            {
                User32Library.SetWindowPos(inputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * ((double)DeviceDpi / 96)), 0, (int)((Width - 45) * ((double)DeviceDpi / 96)), (int)(45 * ((double)DeviceDpi / 96)), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);

            if (ExitModeService.ExitMode.Equals(ExitModeService.ExitModeList[0]))
            {
                // 如果有正在下载的任务，将窗口放到托盘区域
                if (DownloadSchedulerService.GetDownloadSchedulerList().Count > 0)
                {
                    args.Cancel = true;
                    Hide();
                }
                else
                {
                    if (RuntimeHelper.IsElevated && Handle != IntPtr.Zero)
                    {
                        CHANGEFILTERSTRUCT changeFilterStatus = new()
                        {
                            cbSize = Marshal.SizeOf<CHANGEFILTERSTRUCT>()
                        };
                        User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_RESET, in changeFilterStatus);
                    }

                    if (desktopWindowTarget is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;

                    SystemTrayService.MenuItemClick -= OnMenuItemClick;
                    SystemTrayService.MouseDoubleClick -= OnSystemTrayDoubleClick;

                    Current = null;
                    (global::Windows.UI.Xaml.Application.Current as App).Dispose();
                }
            }
            else
            {
                args.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            SetWindowTheme();
            TopMost = TopMostService.TopMostValue;
            desktopWindowTarget = BackdropHelper.InitializeDesktopWindowTarget(this, false);
            SystemTrayService.SetMenuTheme();
            SetWindowBackdrop();

            if (inputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
            {
                User32Library.SetWindowPos(inputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * ((double)DeviceDpi / 96)), 0, (int)((Width - 45) * ((double)DeviceDpi / 96)), (int)(45 * ((double)DeviceDpi / 96)), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 窗体样式发生改变后的事件
        /// </summary>
        protected override void OnStyleChanged(EventArgs args)
        {
            base.OnStyleChanged(args);
            (Content as MainPage).IsWindowMaximizeEnabled = MaximizeBox;
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
                    if (popupRoot.Child is FlyoutPresenter flyoutPresenter && !flyoutPresenter.Name.Equals("DialogFlyout"))
                    {
                        popupRoot.IsOpen = false;
                    }

                    // 关闭菜单浮出控件
                    if (popupRoot.Child as MenuFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                    }

                    // 关闭日期选择器浮出控件
                    if (popupRoot.Child as DatePickerFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
                    }

                    // 关闭时间选择器浮出控件
                    if (popupRoot.Child as TimePickerFlyoutPresenter is not null)
                    {
                        popupRoot.IsOpen = false;
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

            if (Content is not null)
            {
                (Content as MainPage).IsWindowMaximizeEnabled = MaximizeBox;
                (Content as MainPage).IsWindowMaximized = WindowState is FormWindowState.Maximized;

                if (Content.XamlRoot is not null)
                {
                    IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot);
                    foreach (Popup popupRoot in PopupRoot)
                    {
                        // 关闭内容对话框
                        if (popupRoot.Child as ContentDialog is not null)
                        {
                            (popupRoot.Child as ContentDialog).Hide();
                        }

                        // 关闭浮出控件
                        if (popupRoot.Child as FlyoutPresenter is not null)
                        {
                            popupRoot.IsOpen = false;
                        }
                    }
                }

                if (inputNonClientPointerSourceHandle != IntPtr.Zero && Width is not 0)
                {
                    User32Library.SetWindowPos(inputNonClientPointerSourceHandle, IntPtr.Zero, (int)(45 * ((double)DeviceDpi / 96)), 0, (int)((Width - 45) * ((double)DeviceDpi / 96)), (int)(45 * ((double)DeviceDpi / 96)), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
                }
            }
        }

        #endregion 第一部分：窗口类内置需要重载的事件

        #region 第二部分：自定义事件

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            BeginInvoke(() =>
            {
                if (args.PropertyName.Equals(nameof(AlwaysShowBackdropService.AlwaysShowBackdropValue)))
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue;
                    }
                }
                if (args.PropertyName.Equals(nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                    SystemTrayService.SetMenuTheme();
                }
                if (args.PropertyName.Equals(nameof(BackdropService.AppBackdrop)))
                {
                    SetWindowBackdrop();
                }
                if (args.PropertyName.Equals(nameof(TopMostService.TopMostValue)))
                {
                    TopMost = TopMostService.TopMostValue;
                }
            });
        }

        /// <summary>
        /// 托盘图标鼠标单击事件
        /// </summary>
        private void OnMenuItemClick(object sender, EventArgs args)
        {
            if (sender is MenuItem menuItem)
            {
                string tag = Convert.ToString(menuItem.Tag);

                if (tag == nameof(SystemTray.ShowOrHideWindow))
                {
                    if (Visible)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }
                }
                else if (tag == nameof(SystemTray.Exit))
                {
                    (global::Windows.UI.Xaml.Application.Current as App).Dispose();
                }
            }
        }

        /// <summary>
        /// 托盘图标鼠标双击事件
        /// </summary>
        private void OnSystemTrayDoubleClick(object sender, MouseEventArgs args)
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        #endregion 第二部分：自定义事件

        #region 第三部分：窗口过程

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
                        SetWindowTheme();
                        SystemTrayService.SetMenuTheme();
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case (int)WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(m.WParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_MOUSEMENU || sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new global::Windows.Foundation.Point(0, 45),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            return;
                        }
                        break;
                    }
                // 提升权限时允许应用接收拖放消息
                case (int)WindowMessage.WM_DROPFILES:
                    {
                        IntPtr wParam = m.WParam;
                        Task.Run(() =>
                        {
                            List<string> filesList = [];
                            StringBuilder stringBuilder = new(260);
                            uint numFiles = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);
                            for (uint index = 0; index < numFiles; index++)
                            {
                                if (Shell32Library.DragQueryFile(wParam, index, stringBuilder, Convert.ToUInt32(stringBuilder.Capacity) * 2) > 0)
                                {
                                    filesList.Add(stringBuilder.ToString());
                                }
                            }
                            System.Drawing.Point point = new(0, 0);
                            Shell32Library.DragQueryPoint(wParam, ref point);
                            Shell32Library.DragFinish(wParam);
                            BeginInvoke(() =>
                            {
                                (Content as MainPage).SendReceivedFilesList(filesList);
                            });
                        });

                        break;
                    }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
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
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        // HTCAPTION 在标题栏中
                        if (wParam.ToUInt32() is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            System.Drawing.Point clientPoint = PointToClient(MousePosition);
                            FlyoutShowOptions options = new()
                            {
                                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                                ShowMode = FlyoutShowMode.Standard,
                            };

                            if (RightToLeft is RightToLeft.Yes)
                            {
                                if (InfoHelper.SystemVersion.Build >= 22000)
                                {
                                    options.Position = new global::Windows.Foundation.Point((ClientSize.Width - clientPoint.X) / ((double)DeviceDpi / 96), clientPoint.Y / ((double)DeviceDpi / 96));
                                }
                                else
                                {
                                    options.Position = new global::Windows.Foundation.Point(ClientSize.Width - clientPoint.X, clientPoint.Y);
                                }
                            }
                            else
                            {
                                if (InfoHelper.SystemVersion.Build >= 22000)
                                {
                                    options.Position = new global::Windows.Foundation.Point(clientPoint.X / ((double)DeviceDpi / 96), clientPoint.Y / ((double)DeviceDpi / 96));
                                }
                                else
                                {
                                    options.Position = new global::Windows.Foundation.Point(clientPoint.X, clientPoint.Y);
                                }
                            }

                            (Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                        }

                        return IntPtr.Zero;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第三部分：窗口过程

        #region 第四部分：窗口属性设置

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
        public void SetWindowTheme()
        {
            if (Enum.TryParse(ThemeService.AppTheme.Value.ToString(), out ElementTheme theme))
            {
                (Content as MainPage).WindowTheme = theme;
            }
        }

        /// <summary>
        /// 添加窗口背景色
        /// </summary>
        public void SetWindowBackdrop()
        {
            if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[0].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundDefault", false);

                if (systemBackdrop is not null)
                {
                    systemBackdrop?.Dispose();
                    systemBackdrop = null;
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[1].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                if (desktopWindowTarget is not null)
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    systemBackdrop ??= new MicaBackdrop(desktopWindowTarget, Content as FrameworkElement, this)
                    {
                        IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue,
                        Kind = MicaKind.Base
                    };
                    systemBackdrop.InitializeBackdrop();
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[2].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                if (desktopWindowTarget is not null)
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    systemBackdrop ??= new MicaBackdrop(desktopWindowTarget, Content as FrameworkElement, this)
                    {
                        IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue,
                        Kind = MicaKind.BaseAlt
                    };
                    systemBackdrop.InitializeBackdrop();
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[3].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                if (desktopWindowTarget is not null)
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    systemBackdrop ??= new DesktopAcrylicBackdrop(desktopWindowTarget, Content as FrameworkElement, this)
                    {
                        IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue,
                        Kind = DesktopAcrylicKind.Default,
                        UseHostBackdropBrush = true
                    };
                    systemBackdrop.InitializeBackdrop();
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[4].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                if (desktopWindowTarget is not null)
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    systemBackdrop ??= new DesktopAcrylicBackdrop(desktopWindowTarget, Content as FrameworkElement, this)
                    {
                        IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue,
                        Kind = DesktopAcrylicKind.Base,
                        UseHostBackdropBrush = true
                    };
                    systemBackdrop.InitializeBackdrop();
                }
            }
            else if (BackdropService.AppBackdrop.Value.Equals(BackdropService.BackdropList[5].Value))
            {
                VisualStateManager.GoToState(Content as MainPage, "BackgroundTransparent", false);
                if (desktopWindowTarget is not null)
                {
                    if (systemBackdrop is not null)
                    {
                        systemBackdrop?.Dispose();
                        systemBackdrop = null;
                    }

                    systemBackdrop ??= new DesktopAcrylicBackdrop(desktopWindowTarget, Content as FrameworkElement, this)
                    {
                        IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue,
                        Kind = DesktopAcrylicKind.Thin,
                        UseHostBackdropBrush = true
                    };
                    systemBackdrop.InitializeBackdrop();
                }
            }
        }

        #endregion 第四部分：窗口属性设置
    }
}
