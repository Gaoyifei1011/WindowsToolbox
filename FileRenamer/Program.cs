using FileRenamer.Helpers.Root;
using FileRenamer.Services.Controls.Settings;
using FileRenamer.Services.Root;
using FileRenamer.Strings;
using FileRenamer.Views.Forms;
using FileRenamer.WindowsAPI.PInvoke.User32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Shell;
using Windows.Storage;

namespace FileRenamer
{
    /// <summary>
    /// 文件重命名工具 桌面程序
    /// </summary>
    public class Program
    {
        public static App ApplicationRoot { get; private set; }

        public static MainForm MainWindow { get; private set; }

        public static string ErrorFileFolderPath { get; } = ApplicationData.Current.LocalCacheFolder.Path;

        public static string TempFilePath { get; } = Path.Combine(Path.GetTempPath(), "FileRenamer.txt");

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (!RuntimeHelper.IsMSIX)
            {
                Process.Start("explorer.exe", "shell:AppsFolder\\Gaoyifei1011.FileRenamer_pystbwmrmew8c!FileRenamer");
                return;
            }

            if (args.Length is 0)
            {
                CheckProcessState();
            }
            else
            {
                if (args[0] is not "Restart")
                {
                    if (args[0] is "FileName" || args[0] is "ExtensionName" || args[0] is "UpperAndLowerCase" || args[0] is "FileProperties")
                    {
                        if (args.Length is 1)
                        {
                            CheckProcessState(args[0]);
                        }
                        else if (args.Length is 2)
                        {
                            CheckProcessState(string.Format("{0} {1}", args[0], args[1]));
                        }
                    }
                }
            }

            InitializeProgramResources();
            InitializeJumpList();

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
            ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("HelpLink:" + args.Exception.HelpLink);
            stringBuilder.AppendLine("HResult:" + args.Exception.HResult);
            stringBuilder.AppendLine("Message:" + args.Exception.Message);
            stringBuilder.AppendLine("Source:" + args.Exception.Source);
            stringBuilder.AppendLine("StackTrace:" + args.Exception.StackTrace);

            File.AppendAllText(Path.Combine(ErrorFileFolderPath, "ErrorInformation.log"), stringBuilder.ToString());

            DialogResult Result = MessageBox.Show(
                Resources.Title + Environment.NewLine +
                Resources.Content1 + Environment.NewLine +
                Resources.Content2,
                Resources.AppDisplayName,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                );

            // 打开资源管理器存放异常信息的文件目录
            if (Result is DialogResult.OK)
            {
                Process.Start("explorer.exe", ErrorFileFolderPath);
            }
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception exception = args.ExceptionObject as Exception;

            StringBuilder stringBuilder = new StringBuilder();
            if (exception is not null)
            {
                stringBuilder.AppendLine("HelpLink:" + exception.HelpLink);
                stringBuilder.AppendLine("HResult:" + exception.HResult);
                stringBuilder.AppendLine("Message:" + exception.Message);
                stringBuilder.AppendLine("Source:" + exception.Source);
                stringBuilder.AppendLine("StackTrace:" + exception.StackTrace);
            }

            File.AppendAllText(Path.Combine(ErrorFileFolderPath, "ErrorInformation.log"), stringBuilder.ToString());

            DialogResult Result = MessageBox.Show(
                Resources.Title + Environment.NewLine +
                Resources.Content1 + Environment.NewLine +
                Resources.Content2,
                Resources.AppDisplayName,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                );

            // 打开资源管理器存放异常信息的文件目录
            if (Result is DialogResult.OK)
            {
                Process.Start("explorer.exe", ErrorFileFolderPath);
            }
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LanguageService.InitializeLanguage();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageService.AppLanguage.Value.ToString());
            ResourceService.LocalizeReosurce();

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
            ThemeService.InitializeTheme();
            TopMostService.InitializeTopMostValue();

            FileShellMenuService.InitializeFileShellMenu();
        }

        /// <summary>
        /// 初始化跳转菜单列表
        /// </summary>
        private static void InitializeJumpList()
        {
            JumpList jumpList = new JumpList();
            jumpList.ShowFrequentCategory = false;
            jumpList.ShowRecentCategory = false;
            JumpTask fileNameTask = new JumpTask()
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = "FileName",
                Description = Window.FileName,
                IconResourceIndex = 2,
                IconResourcePath = Path.Combine(Environment.SystemDirectory, "Imageres.dll"),
                Title = Window.FileName,
            };
            JumpTask extensionNameTask = new JumpTask()
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = "ExtensionName",
                Description = Window.ExtensionName,
                IconResourceIndex = 247,
                IconResourcePath = Path.Combine(Environment.SystemDirectory, "Imageres.dll"),
                Title = Window.ExtensionName,
            };
            JumpTask upperAndLowerCaseTask = new JumpTask()
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = "UpperAndLowerCase",
                Description = Window.UpperAndLowerCase,
                IconResourceIndex = 118,
                IconResourcePath = Path.Combine(Environment.SystemDirectory, "Imageres.dll"),
                Title = Window.UpperAndLowerCase,
            };
            JumpTask filePropertiesTask = new JumpTask()
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = "FileProperties",
                Description = Window.FileProperties,
                IconResourceIndex = 64,
                IconResourcePath = Path.Combine(Environment.SystemDirectory, "Imageres.dll"),
                Title = Window.FileProperties,
            };
            jumpList.JumpItems.Add(fileNameTask);
            jumpList.JumpItems.Add(extensionNameTask);
            jumpList.JumpItems.Add(upperAndLowerCaseTask);
            jumpList.JumpItems.Add(filePropertiesTask);
            jumpList.Apply();
        }

        private static void CheckProcessState(string arguments = null)
        {
            Process[] FileRenamerProcessList = Process.GetProcessesByName("FileRenamer");

            foreach (Process processItem in FileRenamerProcessList)
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
