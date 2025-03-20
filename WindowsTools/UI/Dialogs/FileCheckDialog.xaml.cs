using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Services.Controls.Download;
using WindowsTools.Services.Root;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 下载文件检查对话框
    /// </summary>
    public sealed partial class FileCheckDialog : ContentDialog
    {
        private readonly string downloadUrl;
        private readonly string downloadFilePath;

        public FileCheckDialog(string url, string saveFilePath)
        {
            InitializeComponent();
            downloadUrl = url;
            downloadFilePath = saveFilePath;
        }

        /// <summary>
        /// 删除本地文件并下载文件
        /// </summary>
        private async void OnPrimaryButtonClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    File.Delete(downloadFilePath);
                    DownloadSchedulerService.CreateDownload(downloadUrl, downloadFilePath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });

            if (!result)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteFileFailed));
            }
        }

        /// <summary>
        /// 打开本地目录
        /// </summary>
        private void OnSecondaryButtonClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(Path.GetDirectoryName(downloadFilePath));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open download file path failed", e);
                }
            });
        }
    }
}
