using System;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Controls.Settings;
using WindowsTools.Services.Root;
using WindowsTools.Services.Shell;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.ComTypes;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace WindowsTools
{
    /// <summary>
    /// Windows 工具箱 桌面程序
    /// </summary>
    public class Program
    {
        private static Guid CLSID_ApplicationActivationManager = new("45BA127D-10A8-46EA-8AB7-56EA9078943C");

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                IApplicationActivationManager applicationActivationManager = (IApplicationActivationManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ApplicationActivationManager));
                applicationActivationManager.ActivateApplication("Gaoyifei1011.WindowsTools_pystbwmrmew8c!WindowsTools", string.Empty, ACTIVATEOPTIONS.AO_NONE, out uint _);
                return;
            }

            InitializeProgramResources();

            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            new App();
            Application.Run(new MainWindow());
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Windows Forms Xaml Islands UI Exception", args.Exception);
            (Windows.UI.Xaml.Application.Current as App).Dispose();
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLevel.Warning, "Background thread Exception", args.ExceptionObject as Exception);
            (Windows.UI.Xaml.Application.Current as App).Dispose();
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Value.ToString());
            ResourceService.LocalizeReosurce();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMostValue();

            DownloadOptionsService.InitializeDownload();
            FileShellMenuService.InitializeFileShellMenu();
            ExitModeService.InitializeExitMode();
            ShellMenuService.InitializeShellMenu();

            DownloadSchedulerService.InitializeDownloadScheduler();
        }
    }
}
