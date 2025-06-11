using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using SwitchTheme.Helpers.Root;
using SwitchTheme.Services.Controls.Settings;
using SwitchTheme.Services.Root;
using SwitchTheme.Views.Windows;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace SwitchTheme
{
    public static class Program
    {
        private static readonly NameValueCollection configurationCollection = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;

        /// <summary>
        /// Windows 工具箱 托盘程序
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                try
                {
                    Process.Start("windowstools:");
                }
                catch (Exception)
                { }
                return;
            }

            bool isExisted = false;
            Process[] processArray = Process.GetProcessesByName("SwitchTheme");

            foreach (Process process in processArray)
            {
                if (process.Id is not 0 && process.MainWindowHandle != IntPtr.Zero)
                {
                    isExisted = true;
                    break;
                }
            }

            if (!isExisted)
            {
                InitializeProgramResources();

                configurationCollection["DpiAwareness"] = "PerMonitorV2";
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += OnThreadException;
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                if (AutoSwitchThemeService.AutoSwitchThemeEnableValue)
                {
                    SystemTrayService.InitializeSystemTray(ResourceService.SystemTrayResource.GetString("Title"), Application.ExecutablePath);
                    new SystemTrayApp();
                    Application.Run(new SystemTrayWindow());
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Windows Forms Xaml Islands UI Exception", args.Exception);
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Background thread Exception", args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Key);

            ThemeService.InitializeTheme();
            AutoSwitchThemeService.InitializeAutoSwitchTheme();
        }
    }
}
