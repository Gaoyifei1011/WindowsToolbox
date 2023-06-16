using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.UI.Controls;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using FileRenamer.WindowsAPI.PInvoke.User32;
using Mile.Xaml;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FileRenamer.Views.Forms
{
    public partial class NameChangeForm : Form
    {
        private int windowWidth = 600;
        private int windowHeight = 400;
        private Graphics graphics;

        public WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public static bool IsOpened { get; private set; } = false;

        private static IntPtr PrivateHandle { get; set; } = IntPtr.Zero;

        public NameChangeControl NameChange { get; private set; } = new NameChangeControl();

        public NameChangeForm()
        {
            InitializeComponent();
            graphics = CreateGraphics();

            Controls.Add(MileXamlHost);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Icon.ExtractAssociatedIcon(string.Format(@"{0}{1}", InfoHelper.GetAppInstalledLocation(), @"FileRenamer.exe"));
            MinimizeBox = false;
            MaximizeBox = false;
            Size = new Size(Convert.ToInt32(windowWidth * graphics.DpiX / 96.0), Convert.ToInt32(windowHeight * graphics.DpiX / 96.0));
            StartPosition = FormStartPosition.Manual;
            Left = Program.MainWindow.Left + (Program.MainWindow.Width - Width) / 2;
            Top = Program.MainWindow.Top + (Program.MainWindow.Height - Height) / 2;
            Text = ResourceService.GetLocalized("NameChange/Title");
            graphics.Dispose();

            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = NameChange;
            NameChange.ViewModel.WindowHandle = Handle;
        }

        /// <summary>
        /// 关闭窗口时释放资源
        /// </summary>
        protected override void OnClosed(EventArgs args)
        {
            base.OnClosed(args);
            Program.MainWindow.MessageReceived -= OnMessageReceived;
            BackdropService.BackdropChanged -= OnBackdropChanged;
            ThemeService.ThemeChanged -= OnThemeChanged;
            IsOpened = false;
            PrivateHandle = IntPtr.Zero;
            Dispose();
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            NameChange.ViewModel.SetAppTheme();
            SetWindowBackdrop();

            Program.MainWindow.MessageReceived += OnMessageReceived;
            BackdropService.BackdropChanged += OnBackdropChanged;
            ThemeService.ThemeChanged += OnThemeChanged;
        }

        /// <summary>
        /// 当前显示窗体的显示设备上的 DPI 设置更改时发生
        /// </summary>
        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);

            Size = new Size(
                Convert.ToInt32(windowWidth * args.DeviceDpiNew / 96.0),
                Convert.ToInt32(windowHeight * args.DeviceDpiNew / 96.0)
                );
        }

        /// <summary>
        /// 应用背景色发生改变时，修改当前窗口的背景色
        /// </summary>
        private async void OnBackdropChanged()
        {
            await MileXamlHost.Child.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, SetWindowBackdrop);
        }

        /// <summary>
        /// 应用主题值发生改变时，修改当前窗口的主题
        /// </summary>
        private async void OnThemeChanged()
        {
            await MileXamlHost.Child.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, NameChange.ViewModel.SetAppTheme);
        }

        /// <summary>
        /// 处理 Windows 消息
        /// </summary>
        private async void OnMessageReceived(Message m)
        {
            switch (m.Msg)
            {
                // 系统设置发生变化时的消息
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        await MileXamlHost.Child.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, NameChange.ViewModel.SetAppTheme);
                        break;
                    }
            }
        }

        public new void Show()
        {
            if (!IsOpened)
            {
                IsOpened = true;
                PrivateHandle = Handle;
                base.Show();
            }
            else
            {
                User32Library.SetForegroundWindow(PrivateHandle);
                Dispose();
            }
        }

        /// <summary>
        /// 设置当前窗口的背景色
        /// </summary>
        private void SetWindowBackdrop()
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
