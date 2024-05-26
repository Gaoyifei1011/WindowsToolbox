using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Services.Controls.Download;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;

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
        private void OnPrimaryButtonClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            UnreferenceHelper.Unreference(sender);
            UnreferenceHelper.Unreference(args);

            Task.Run(() =>
            {
                try
                {
                    File.Delete(downloadFilePath);
                    DownloadSchedulerService.CreateDownload(downloadUrl, downloadFilePath);
                }
                catch (Exception)
                {
                    MainWindow.Current.BeginInvoke(() =>
                    {
                        TeachingTipHelper.Show(new OperationResultTip(OperationKind.DeleteFileFailed));
                    });
                    Process.Start(Path.GetDirectoryName(downloadFilePath));
                }
            });
        }

        /// <summary>
        /// 打开本地目录
        /// </summary>
        private void OnSecondaryButtonClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            UnreferenceHelper.Unreference(sender);
            UnreferenceHelper.Unreference(args);

            Task.Run(() =>
            {
                Process.Start(Path.GetDirectoryName(downloadFilePath));
            });
        }
    }
}
