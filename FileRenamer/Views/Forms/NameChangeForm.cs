using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.UI.Dialogs;
using FileRenamer.WindowsAPI.PInvoke.DwmApi;
using FileRenamer.WindowsAPI.PInvoke.User32;
using FileRenamer.WindowsAPI.PInvoke.Uxtheme;
using Mile.Xaml;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

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

        public NameChangeDialog NameChange { get; private set; } = new NameChangeDialog();

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
            Text = ResourceService.GetLocalized("Dialog/NameChangeTitle");
            graphics.Dispose();

            MileXamlHost.AutoSize = true;
            MileXamlHost.Dock = DockStyle.Fill;
            MileXamlHost.Child = NameChange;
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
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);
            AllowTransparency = false;
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
        /// 窗体创建句柄时初始化应用程序设置
        /// </summary>
        protected override void OnHandleCreated(EventArgs args)
        {
            base.OnHandleCreated(args);

            NameChange.ViewModel.WindowHandle = Handle;
            SetAppTheme();
            SetWindowBackdrop();

            Program.MainWindow.MessageReceived += OnMessageReceived;
            BackdropService.BackdropChanged += OnBackdropChanged;
            ThemeService.ThemeChanged += OnThemeChanged;
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
            await MileXamlHost.Child.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, SetAppTheme);
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
                        await MileXamlHost.Child.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, SetAppTheme);
                        RefreshWindowState();
                        break;
                    }
            }
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
                        if (NameChange.TitlebarMenuFlyout.IsOpen)
                        {
                            NameChange.TitlebarMenuFlyout.Hide();
                        }
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
                            NameChange.TitlebarMenuFlyout.ShowAt(null, options);
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
                        NameChange.TitlebarMenuFlyout.ShowAt(null, options);

                        return;
                    }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
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

                RefreshWindowState();
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[1].InternalName)
            {
                int micaBackdrop = 2;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));

                RefreshWindowState();
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[2].InternalName)
            {
                int micaAltBackdrop = 4;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaAltBackdrop, Marshal.SizeOf(typeof(int)));

                RefreshWindowState();
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[3].InternalName)
            {
                int acrylicBackdrop = 3;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref acrylicBackdrop, Marshal.SizeOf(typeof(int)));

                RefreshWindowState();
            }
        }

        /// <summary>
        /// 设置当前窗口的主题色
        /// </summary>
        public void SetAppTheme()
        {
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                if (Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    int useLightMode = 0;
                    DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                    UxthemeLibrary.FlushMenuThemes();

                    RefreshWindowState();
                }
                else
                {
                    int useDarkMode = 1;
                    DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
                    UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                    UxthemeLibrary.FlushMenuThemes();

                    RefreshWindowState();
                }
            }
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                int useLightMode = 0;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                UxthemeLibrary.FlushMenuThemes();

                RefreshWindowState();
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                int useDarkMode = 1;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                UxthemeLibrary.FlushMenuThemes();

                RefreshWindowState();
            }
        }

        /// <summary>
        /// 当主题色或背景色发生改变时，刷新窗体的状态
        /// </summary>
        public void RefreshWindowState()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Windows.UI.Xaml.Application.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Light)
                    {
                        TransparencyKey = BackColor = Color.White;
                        AllowTransparency = false;
                    }
                    else
                    {
                        TransparencyKey = BackColor = Color.Black;
                        AllowTransparency = false;
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    TransparencyKey = BackColor = Color.White;
                    AllowTransparency = false;
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    TransparencyKey = BackColor = Color.Black;
                    AllowTransparency = false;
                }
            }
            else
            {
                AllowTransparency = true;
                BackColor = Color.FromArgb(255, 255, 254);
                TransparencyKey = Color.FromArgb(255, 255, 254);
            }
        }
    }
}
