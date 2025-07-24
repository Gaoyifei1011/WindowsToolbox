using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.Pages;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace PowerToolbox.Views.Windows
{
    /// <summary>
    /// 摸鱼窗口
    /// </summary>
    public class LoafWindow : Form
    {
        private readonly bool _blockAllKeys = false;
        private readonly bool _lockScreenAutomaticly = false;
        private readonly Container container = new();
        private readonly DesktopWindowXamlSource desktopWindowXamlSource = new();

        private readonly nint hwndXamlIsland;
        private nint hHook = nint.Zero;
        private HOOKPROC keyBoardHookProc;

        public UIElement Content { get; set; }

        public static LoafWindow Current { get; private set; }

        public LoafWindow(UpdateKind updatingKind, TimeSpan duration, bool blockAllKeys, bool lockScreenAutomaticly)
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Content = new SimulateUpdatePage(updatingKind, duration);

            desktopWindowXamlSource.Content = Content;
            IDesktopWindowXamlSourceNative2 desktopWindowXamlSourceNative = desktopWindowXamlSource as IDesktopWindowXamlSourceNative2;
            desktopWindowXamlSourceNative.AttachToWindow(Handle);
            desktopWindowXamlSourceNative.GetWindowHandle(out hwndXamlIsland);
            desktopWindowXamlSource.TakeFocusRequested += OnTakeFocusRequested;

            RightToLeft = LanguageService.RightToLeft;
            RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            WindowState = FormWindowState.Maximized;

            _blockAllKeys = blockAllKeys;
            _lockScreenAutomaticly = lockScreenAutomaticly;
            Cursor.Hide();

            int exStyle = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_EXSTYLE);
            SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_EXSTYLE, (nint)(exStyle & ~0x00040000 | 0x00000080));
            SystemSleepHelper.PreventForCurrentThread();
            StartHook();
            Show();
        }

        /// <summary>
        /// 关闭窗体后发生的事件
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs args)
        {
            base.OnFormClosed(args);
            Current = null;
            desktopWindowXamlSource.Dispose();
            desktopWindowXamlSource.TakeFocusRequested -= OnTakeFocusRequested;
        }

        /// <summary>
        /// 窗口大小改变时发生的事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs args)
        {
            base.OnSizeChanged(args);
            User32Library.SetWindowPos(hwndXamlIsland, nint.Zero, 0, 0, Width, Height, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && container is not null)
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// 例如，当主机桌面应用程序收到从 DesktopWindowXamlSource 对象 (获取焦点的请求时发生，用户位于 DesktopWindowXamlSource 中的最后一个可聚焦元素上，然后按 Tab) 。
        /// </summary>
        private void OnTakeFocusRequested(DesktopWindowXamlSource sender, DesktopWindowXamlSourceTakeFocusRequestedEventArgs args)
        {
            XamlSourceFocusNavigationReason reason = args.Request.Reason;

            if (reason < XamlSourceFocusNavigationReason.Left)
            {
                sender.NavigateFocus(args.Request);
            }
        }

        /// <summary>
        /// 添加钩子
        /// </summary>
        private void StartHook()
        {
            try
            {
                // 安装键盘钩子
                if (hHook.Equals(nint.Zero))
                {
                    keyBoardHookProc = new HOOKPROC(OnKeyboardHookProc);

                    hHook = User32Library.SetWindowsHookEx(HOOKTYPE.WH_KEYBOARD_LL, keyBoardHookProc, Process.GetCurrentProcess().MainModule.BaseAddress, 0);

                    //如果设置钩子失败.
                    if (hHook.Equals(nint.Zero))
                    {
                        StopHook();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(LoafWindow), nameof(StartHook), 1, e);
            }
        }

        /// <summary>
        /// 取消钩子
        /// </summary>
        private void StopHook()
        {
            try
            {
                bool unHookResult = true;
                if (!hHook.Equals(nint.Zero))
                {
                    unHookResult = User32Library.UnhookWindowsHookEx(hHook);
                }

                if (!unHookResult)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(LoafWindow), nameof(StopHook), 1, e);
            }
        }

        /// <summary>
        /// 自定义钩子消息处理
        /// </summary>
        public nint OnKeyboardHookProc(int nCode, nuint wParam, nint lParam)
        {
            // 处理键盘钩子消息
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbdllHookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                // Esc 键，退出摸鱼
                if (kbdllHookStruct.vkCode is Keys.Escape)
                {
                    StopLoaf();
                    return new nint(1);
                }

                // 屏蔽所有键盘按键
                if (_blockAllKeys)
                {
                    // 左 Windows 徽标键
                    if (kbdllHookStruct.vkCode is Keys.LWin)
                    {
                        return new nint(1);
                    }

                    // 右 Windows 徽标键
                    if (kbdllHookStruct.vkCode is Keys.LWin)
                    {
                        return new nint(1);
                    }

                    // Ctrl 和 Esc 组合
                    if (kbdllHookStruct.vkCode is Keys.Escape && ModifierKeys is Keys.Control)
                    {
                        return new nint(1);
                    }

                    // Alt 和 F4 组合
                    if (kbdllHookStruct.vkCode is Keys.F4 && ModifierKeys is Keys.Alt)
                    {
                        return new nint(1);
                    }

                    // Alt 和 Tab 组合
                    if (kbdllHookStruct.vkCode is Keys.Tab && ModifierKeys is Keys.Alt)
                    {
                        return new nint(1);
                    }

                    // Ctrl Shift Esc 组合
                    if (kbdllHookStruct.vkCode is Keys.Escape && ModifierKeys is (Keys.Control | Keys.Shift))
                    {
                        return new nint(1);
                    }

                    // Alt 和 Space 组合
                    if (kbdllHookStruct.vkCode is Keys.Space && ModifierKeys is Keys.Alt)
                    {
                        return new nint(1);
                    }
                }
            }
            return User32Library.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 停止摸鱼
        /// </summary>
        public void StopLoaf()
        {
            Cursor.Show();
            StopHook();
            (Content as SimulateUpdatePage).StopSimulateUpdate();
            if (_lockScreenAutomaticly)
            {
                User32Library.LockWorkStation();
            }
            // 恢复此线程曾经阻止的系统休眠和屏幕关闭。
            SystemSleepHelper.RestoreForCurrentThread();
            Close();
            User32Library.keybd_event(Keys.LWin, 0, KEYEVENTFLAGS.KEYEVENTF_KEYDOWN, nuint.Zero);
            User32Library.keybd_event(Keys.D, 0, KEYEVENTFLAGS.KEYEVENTF_KEYDOWN, nuint.Zero);
            User32Library.keybd_event(Keys.D, 0, KEYEVENTFLAGS.KEYEVENTF_KEYUP, nuint.Zero);
            User32Library.keybd_event(Keys.LWin, 0, KEYEVENTFLAGS.KEYEVENTF_KEYUP, nuint.Zero);
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex)
        {
            return nint.Size is 8 ? User32Library.GetWindowLongPtr(hWnd, nIndex) : User32Library.GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static nint SetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex, nint dwNewLong)
        {
            return nint.Size is 8 ? User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong) : User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }
}
