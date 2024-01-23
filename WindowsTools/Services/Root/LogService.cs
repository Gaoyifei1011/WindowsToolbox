using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WindowsTools.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly object logLock = new object();

        private static bool isInitialized = false;

        private static string logName = Assembly.GetExecutingAssembly().GetName().Name;

        private static string unknown = "unknown";

        private static string LogFolderPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Logs");

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(LogFolderPath))
                {
                    Directory.CreateDirectory(LogFolderPath);
                }
                isInitialized = true;
            }
            catch (Exception)
            {
                isInitialized = false;
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(EventLogEntryType logType, string logContent, StringBuilder logBuilder)
        {
            if (isInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(LogFolderPath, string.Format("{0}_{1}.log", logName, DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}{6}{7}{8}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogType",
                                    Convert.ToString(logType),
                                    Environment.NewLine,
                                    "LogContent:",
                                    logContent,
                                    Environment.NewLine,
                                    logBuilder,
                                    Environment.NewLine)
                                );
                        }
                    });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(EventLogEntryType logType, string logContent, Exception exception)
        {
            if (isInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        StringBuilder exceptionBuilder = new StringBuilder();
                        exceptionBuilder.Append("LogContent:");
                        exceptionBuilder.AppendLine(logContent);
                        exceptionBuilder.Append("HelpLink:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("Message:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("HResult:");
                        exceptionBuilder.AppendLine(exception.HResult.ToString());
                        exceptionBuilder.Append("Source:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("StackTrace:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(LogFolderPath, string.Format("{0}_{1}.log", logName, DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogType",
                                    Convert.ToString(logType),
                                    Environment.NewLine,
                                    exceptionBuilder.ToString(),
                                    Environment.NewLine)
                                );
                        }
                    });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static void OpenLogFolder()
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    Process.Start(LogFolderPath);
                });
            }
        }

        /// <summary>
        /// 清除所有的日志文件
        /// </summary>
        public static bool ClearLog()
        {
            try
            {
                Task.Run(() =>
                {
                    string[] logFiles = Directory.GetFiles(LogFolderPath, "*.log");
                    foreach (string logFile in logFiles)
                    {
                        File.Delete(logFile);
                    }
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
