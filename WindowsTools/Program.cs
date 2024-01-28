using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.User32;

namespace WindowsTools
{
    /// <summary>
    /// Windows 工具箱 桌面程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (!RuntimeHelper.IsMSIX)
            {
                Process.Start("explorer.exe", "shell:AppsFolder\\Gaoyifei1011.WindowsTools_pystbwmrmew8c!WindowsTools");
                return;
            }

            if (args.Length is 0)
            {
                CheckProcessState();
            }

            InitializeProgramResources();

            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ApplicationExit += OnApplicationExit;
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            new App();
            Application.Run(new MainWindow());
        }

        /// <summary>
        /// 在应用程序即将关闭时发生
        /// </summary>
        private static void OnApplicationExit(object sender, EventArgs args)
        {
            (Windows.UI.Xaml.Application.Current as App).Dispose();
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            LogService.WriteLog(EventLogEntryType.Warning, "Windows Forms Xaml Islands UI Exception", args.Exception);
            (Windows.UI.Xaml.Application.Current as App).Dispose();
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLogEntryType.Warning, "Background thread Exception", args.ExceptionObject as Exception);
            (Windows.UI.Xaml.Application.Current as App).Dispose();
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LocalSettingsService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Value.ToString());
            ResourceService.LocalizeReosurce();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMostValue();

            FileShellMenuService.InitializeFileShellMenu();
        }

        private static void CheckProcessState(string arguments = null)
        {
            Process[] WindowsToolsProcessList = Process.GetProcessesByName("WindowsTools");

            foreach (Process processItem in WindowsToolsProcessList)
            {
                if (processItem.MainWindowHandle != IntPtr.Zero)
                {
                    COPYDATASTRUCT copyDataStruct = new COPYDATASTRUCT();
                    if (string.IsNullOrEmpty(arguments))
                    {
                        copyDataStruct.dwData = IntPtr.Zero;
                        copyDataStruct.cbData = Encoding.Default.GetBytes("AppIsRunning").Length + 1;
                        copyDataStruct.lpData = "AppIsRunning";
                    }
                    else
                    {
                        copyDataStruct.dwData = IntPtr.Zero;
                        copyDataStruct.cbData = Encoding.Default.GetBytes(arguments).Length + 1;
                        copyDataStruct.lpData = arguments;
                    }

                    IntPtr ptrCopyDataStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));
                    Marshal.StructureToPtr(copyDataStruct, ptrCopyDataStruct, false);
                    User32Library.SendMessage(processItem.MainWindowHandle, WindowMessage.WM_COPYDATA, 0, ptrCopyDataStruct);
                    Marshal.FreeHGlobal(ptrCopyDataStruct);
                    User32Library.SetForegroundWindow(processItem.MainWindowHandle);

                    Environment.Exit(0);
                }
            }
        }
    }
}
