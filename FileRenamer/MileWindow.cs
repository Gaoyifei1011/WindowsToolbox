using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
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

        private WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public MainPage MainPage { get; private set; } = new MainPage();

        public MileWindow()
        {
            InitializeComponent();

            Graphics graphics = CreateGraphics();
            Size = new Size(Convert.ToInt32(windowWidth * graphics.DpiX / 96.0), Convert.ToInt32(windowHeight * graphics.DpiX / 96.0));
            Text = ResourceService.GetLocalized("Resources/AppDisplayName");
            Icon = Icon.ExtractAssociatedIcon(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), @"FileRenamer.exe"));
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(30, 60, 90);
            TransparencyKey = Color.FromArgb(30, 60, 90);

            Controls.Add(MileXamlHost);
            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = MainPage;

            Load += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs args)
        {
            ThemeService.SetWindowTheme();
            BackdropService.SetAppBackdrop();
            TopMostService.SetAppTopMost();
        }

        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            MinimumSize = new Size(
                Convert.ToInt32(windowWidth * args.DeviceDpiNew / 96.0),
                Convert.ToInt32(windowHeight * args.DeviceDpiNew / 96.0)
                );
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 窗口移动时的事件
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
            }

            base.WndProc(ref m);
        }
    }
}
