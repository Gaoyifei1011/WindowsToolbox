using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.UI.Dialogs;
using FileRenamer.Views.Pages;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using FileRenamer.WindowsAPI.PInvoke.User32;
using Mile.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer.Views.Forms
{
    public partial class MainForm : Form
    {
        private int windowWidth = 1000;
        private int windowHeight = 700;
        private Graphics graphics;

        public WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public event Action<Message> MessageReceived;

        public MainPage MainPage { get; private set; } = new MainPage();

        public MainForm()
        {
            InitializeComponent();
            graphics = CreateGraphics();

            Controls.Add(MileXamlHost);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Icon.ExtractAssociatedIcon(string.Format(@"{0}{1}", InfoHelper.GetAppInstalledLocation(), @"FileRenamer.exe"));
            MaximizeBox = false;
            Size = new Size(Convert.ToInt32(windowWidth * graphics.DpiX / 96.0), Convert.ToInt32(windowHeight * graphics.DpiX / 96.0));
            StartPosition = FormStartPosition.CenterParent;
            Text = ResourceService.GetLocalized("Resources/AppDisplayName");
            graphics.Dispose();

            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = MainPage;
        }

        /// <summary>
        /// 当前显示窗体的显示设备上的 DPI 设置更改时发生
        /// </summary>
        protected override async void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);

            Size = new Size(
                Convert.ToInt32(windowWidth * args.DeviceDpiNew / 96.0),
                Convert.ToInt32(windowHeight * args.DeviceDpiNew / 96.0)
                );

            await new DPIChangedNotifyDialog().ShowAsync();
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            ThemeService.SetWindowTheme();
            BackdropService.SetAppBackdrop(Handle);

            MainPage.ViewModel.SetAppTheme();
        }

        /// <summary>
        /// 窗体大小发生改变时响应的事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs args)
        {
            base.OnSizeChanged(args);
        }

        /// <summary>
        /// 处理 Windows 消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 窗口移动时的消息
                case (int)WindowMessage.WM_MOVE:
                    {
                        if (MainPage.TitlebarMenuFlyout.IsOpen)
                        {
                            MainPage.TitlebarMenuFlyout.Hide();
                        }
                        IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(Program.MainWindow.MainPage.XamlRoot);
                        foreach (Popup popup in PopupRoot)
                        {
                            if (popup.Child as MenuFlyoutPresenter is not null)
                            {
                                popup.IsOpen = false;
                                break;
                            }

                            if (popup.Child as Canvas is not null)
                            {
                                popup.IsOpen = false;
                                break;
                            }

                            if (popup.Child as TimePickerFlyoutPresenter is not null)
                            {
                                popup.IsOpen = false;
                                break;
                            }

                            if (popup.Child as DatePickerFlyoutPresenter is not null)
                            {
                                popup.IsOpen = false;
                                break;
                            }
                        }
                        break;
                    }
                // 系统设置发生变化时的消息
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        MainPage.ViewModel.SetAppTheme();
                        MessageReceived?.Invoke(m);
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case (int)WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(m.WParam.ToInt32() & 0xFFF0);

                        if (sysCommand == SystemCommand.SC_MOUSEMENU || sysCommand == SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Windows.Foundation.Point(0, 0);
                            options.ShowMode = FlyoutShowMode.Standard;
                            MainPage.TitlebarMenuFlyout.ShowAt(null, options);
                            return;
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case (int)WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        Point ms = MousePosition;
                        FlyoutShowOptions options = new FlyoutShowOptions();
                        options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                        options.ShowMode = FlyoutShowMode.Standard;
                        options.Position = InfoHelper.SystemVersion.Build >= 22000 ? new Windows.Foundation.Point(DPICalcHelper.ConvertPixelToEpx(Handle, ms.X - Location.X - 8), DPICalcHelper.ConvertPixelToEpx(Handle, ms.Y - Location.Y - 32)) : new Windows.Foundation.Point(ms.X - Location.X, ms.Y - Location.Y - 32);
                        MainPage.TitlebarMenuFlyout.ShowAt(null, options);
                        return;
                    }
            }

            base.WndProc(ref m);
        }

        public void SetWindowBackdrop()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                int noBackdrop = 1;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref noBackdrop, Marshal.SizeOf(typeof(int)));
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[1].InternalName)
            {
                int micaBackdrop = 2;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[2].InternalName)
            {
                int micaAltBackdrop = 4;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaAltBackdrop, Marshal.SizeOf(typeof(int)));
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[3].InternalName)
            {
                int acrylicBackdrop = 3;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref acrylicBackdrop, Marshal.SizeOf(typeof(int)));
            }
        }
    }
}
