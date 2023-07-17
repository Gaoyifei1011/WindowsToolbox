using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.Views.Forms;
using FileRenamer.WindowsAPI.PInvoke.User32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRenamer
{
    /// <summary>
    /// 文件重命名工具 桌面程序
    /// </summary>
    public class Program
    {
        public static Mutex AppMutex = null;

        public static App ApplicationRoot { get; private set; }

        public static MainForm MainWindow { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureInfo.CurrentCulture.Parent.Parent.Name);

            bool isExists = Mutex.TryOpenExisting(Assembly.GetExecutingAssembly().GetName().Name, out AppMutex);
            if (isExists && AppMutex is not null)
            {
                Process[] FileRenamerProcessList = Process.GetProcessesByName("FileRenamer");

                foreach (Process processItem in FileRenamerProcessList)
                {
                    if (processItem.MainWindowHandle != IntPtr.Zero)
                    {
                        User32Library.SetForegroundWindow(processItem.MainWindowHandle);
                        processItem.Dispose();
                    }
                }
                return;
            }
            else
            {
                AppMutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name);
            }

            InitializeProgramResourcesAsync().Wait();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ApplicationExit += OnApplicationExit;
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ApplicationRoot = new App();
            ResourceDictionaryHelper.InitializeResourceDictionary();

            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow = new MainForm();
            Application.Run(MainWindow);
        }

        /// <summary>
        /// 在应用程序即将关闭时发生
        /// </summary>
        private static void OnApplicationExit(object sender, EventArgs args)
        {
            ApplicationRoot.CloseApp(false);
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendLine("HelpLink:" + args.Exception.HelpLink);
            stringBuilder1.AppendLine("HResult:" + args.Exception.HResult);
            stringBuilder1.AppendLine("Message:" + args.Exception.Message);
            stringBuilder1.AppendLine("Source:" + args.Exception.Source);
            stringBuilder1.AppendLine("StackTrace:" + args.Exception.StackTrace);

            DialogResult Result = MessageBox.Show(
                ResourceService.GetLocalized("Resources/Title") + Environment.NewLine +
                ResourceService.GetLocalized("Resources/Content1") + Environment.NewLine +
                ResourceService.GetLocalized("Resources/Content2"),
                ResourceService.GetLocalized("Resources/AppDisplayName"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                );

            // 复制异常信息到剪贴板
            if (Result == DialogResult.OK)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("HelpLink:" + args.Exception.HelpLink);
                stringBuilder.AppendLine("HResult:" + args.Exception.HResult);
                stringBuilder.AppendLine("Message:" + args.Exception.Message);
                stringBuilder.AppendLine("Source:" + args.Exception.Source);
                stringBuilder.AppendLine("StackTrace:" + args.Exception.StackTrace);

                CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            }

            return;
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception exception1 = args.ExceptionObject as Exception;

            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendLine("HelpLink:" + exception1.HelpLink);
            stringBuilder1.AppendLine("HResult:" + exception1.HResult);
            stringBuilder1.AppendLine("Message:" + exception1.Message);
            stringBuilder1.AppendLine("Source:" + exception1.Source);
            stringBuilder1.AppendLine("StackTrace:" + exception1.StackTrace);

            DialogResult Result = MessageBox.Show(
                ResourceService.GetLocalized("Resources/Title") + Environment.NewLine +
                ResourceService.GetLocalized("Resources/Content1") + Environment.NewLine +
                ResourceService.GetLocalized("Resources/Content2"),
                ResourceService.GetLocalized("Resources/AppDisplayName"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                );

            // 复制异常信息到剪贴板
            if (Result == DialogResult.OK)
            {
                Exception exception = args.ExceptionObject as Exception;
                if (exception is not null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("HelpLink:" + exception.HelpLink);
                    stringBuilder.AppendLine("HResult:" + exception.HResult);
                    stringBuilder.AppendLine("Message:" + exception.Message);
                    stringBuilder.AppendLine("Source:" + exception.Source);
                    stringBuilder.AppendLine("StackTrace:" + exception.StackTrace);

                    CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
                }
            }
            return;
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeProgramResourcesAsync()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                if (!File.Exists(ConfigService.UnPackagedConfigFile))
                {
                    File.Create(ConfigService.UnPackagedConfigFile);
                }
            }
            await LanguageService.InitializeLanguageAsync();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResourceService.LocalizeReosurce();

            await BackdropService.InitializeBackdropAsync();
            await ThemeService.InitializeAsync();
        }
    }
}
