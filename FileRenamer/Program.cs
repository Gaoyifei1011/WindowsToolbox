using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Root;
using FileRenamer.WindowsAPI.PInvoke.Comctl32;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRenamer
{
    public static class Program
    {
        public static Mutex AppMutex = null;

        public static App ApplicationRoot { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            bool isExists = Mutex.TryOpenExisting(Assembly.GetExecutingAssembly().GetName().Name, out AppMutex);
            if (isExists && AppMutex is not null) { return; }
            else
            {
                AppMutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name);
            }

            InitializeProgramResourcesAsync().Wait();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ApplicationRoot = new App();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MileWindow());
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            Comctl32Library.TaskDialog(
                    MileWindow.Current.Handle,
                    IntPtr.Zero,
                    ResourceService.GetLocalized("Resources/AppDisplayName"),
                    ResourceService.GetLocalized("MessageInfo/Title"),
                    ResourceService.GetLocalized("MessageInfo/Content1") + Environment.NewLine + ResourceService.GetLocalized("MessageInfo/Content2"),
                    TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON | TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CANCEL_BUTTON,
                    TASKDIALOGICON.TD_SHIELD_ERROR_RED_BAR,
                    out TaskDialogResult Result
                    );

            // 复制异常信息到剪贴板
            if (Result == TaskDialogResult.IDOK)
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
            return;
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeProgramResourcesAsync()
        {
            await LanguageService.InitializeLanguageAsync();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResourceService.LocalizeReosurce();

            // 初始化应用版本和系统版本信息
            InfoHelper.InitializeAppVersion();
            InfoHelper.InitializeSystemVersion();

            await ThemeService.InitializeAsync();
        }
    }
}
