﻿using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThemeSwitch.WindowsAPI.PInvoke.Shell32;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace ThemeSwitch.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly object logLock = new();
        private static readonly string logName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string unknown = "unknown";

        private static bool isInitialized = false;
        private static DirectoryInfo logDirectory;
        private static Guid FOLDERID_LocalAppData = new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            Shell32Library.SHGetKnownFolderPath(FOLDERID_LocalAppData, KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, IntPtr.Zero, out string localAppdataPath);

            if (!string.IsNullOrEmpty(localAppdataPath))
            {
                try
                {
                    if (Directory.Exists(localAppdataPath))
                    {
                        string logFolderPath = Path.Combine(localAppdataPath, "Logs");
                        logDirectory = Directory.CreateDirectory(logFolderPath);
                        isInitialized = true;
                    }
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
        public static void WriteLog(EventLevel logLevel, string logContent, Exception exception)
        {
            if (isInitialized)
            {
                Task.Run(() =>
                {
                    try
                    {
                        StringBuilder exceptionBuilder = new();
                        exceptionBuilder.Append("LogContent:");
                        exceptionBuilder.AppendLine(logContent);
                        exceptionBuilder.Append("HelpLink:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("Message:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("HResult:");
                        exceptionBuilder.AppendLine(Convert.ToString(exception.HResult));
                        exceptionBuilder.Append("Source:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("StackTrace:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(logDirectory.FullName, string.Format("{0}_{1}.log", logName, DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogType",
                                    Convert.ToString(logLevel),
                                    Environment.NewLine,
                                    Convert.ToString(exceptionBuilder),
                                    Environment.NewLine)
                                );
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                });
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
                    try
                    {
                        Process.Start(logDirectory.FullName);
                    }
                    catch (Exception e)
                    {
                        WriteLog(EventLevel.Error, "Open log folder failed", e);
                    }
                });
            }
        }
    }
}
