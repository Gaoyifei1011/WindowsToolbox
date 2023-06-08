using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.UI.Dialogs;
using FileRenamer.Views.Pages;
using FileRenamer.WindowsAPI.PInvoke.User32;
using Mile.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer
{
    public partial class MileWindow : Form
    {
        private int windowWidth = 1000;
        private int windowHeight = 700;
        private Graphics graphics;

        public WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public MainPage MainPage { get; private set; } = new MainPage();

        public MileWindow()
        {
            InitializeComponent();
            graphics = CreateGraphics();

            BackColor = Color.FromArgb(30, 60, 90);
            Controls.Add(MileXamlHost);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Icon.ExtractAssociatedIcon(string.Format(@"{0}{1}", InfoHelper.GetAppInstalledLocation(), @"FileRenamer.exe"));
            MaximizeBox = false;
            Size = new Size(Convert.ToInt32(windowWidth * graphics.DpiX / 96.0), Convert.ToInt32(windowHeight * graphics.DpiX / 96.0));
            StartPosition = FormStartPosition.CenterParent;
            Text = ResourceService.GetLocalized("Resources/AppDisplayName");
            TransparencyKey = Color.FromArgb(30, 60, 90);
            graphics.Dispose();

            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = MainPage;
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

            MainPage.ViewModel.SetAppTheme();
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
        /// 处理 Windows 消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 窗口移动时的消息
                case (int)WindowMessage.WM_MOVE:
                    {
                        IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(Program.MainWindow.MainPage.XamlRoot);
                        foreach (Popup popup in PopupRoot)
                        {
                            if (popup.Child as MenuFlyoutPresenter != null)
                            {
                                popup.IsOpen = false;
                            }

                            if (popup.Child as Canvas != null)
                            {
                                popup.IsOpen = false;
                            }
                        }
                        break;
                    }
                // 系统设置发生变化时的消息
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        MainPage.ViewModel.SetAppTheme();
                        break;
                    }
            }

            base.WndProc(ref m);
        }
    }
}
