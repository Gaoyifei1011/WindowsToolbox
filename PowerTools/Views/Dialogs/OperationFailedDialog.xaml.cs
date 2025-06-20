using PowerTools.Helpers.Root;
using PowerTools.Models;
using PowerTools.Services.Root;
using PowerTools.Views.NotificationTips;
using PowerTools.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerTools.Views.Dialogs
{
    /// <summary>
    /// 错误信息列表对话框
    /// </summary>
    public sealed partial class OperationFailedDialog : ContentDialog
    {
        private readonly string ExceptionCodeString = ResourceService.DialogResource.GetString("ExceptionCode");
        private readonly string ExceptionMessageString = ResourceService.DialogResource.GetString("ExceptionMessage");
        private readonly string FileNameCopyString = ResourceService.DialogResource.GetString("FileNameCopy");
        private readonly string FilePathCopyString = ResourceService.DialogResource.GetString("FilePathCopy");

        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public OperationFailedDialog(List<OperationFailedModel> operationFailedCollection)
        {
            InitializeComponent();

            foreach (OperationFailedModel operationFailedItem in operationFailedCollection)
            {
                OperationFailedCollection.Add(operationFailedItem);
            }
        }

        /// <summary>
        /// 复制异常信息
        /// </summary>
        private async void OnCopyExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OperationFailedModel operationFailedItem)
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();
                    stringBuilder.Append(FileNameCopyString);
                    stringBuilder.AppendLine(operationFailedItem.FileName);
                    stringBuilder.Append(FilePathCopyString);
                    stringBuilder.AppendLine(operationFailedItem.FilePath);
                    stringBuilder.Append(ExceptionMessageString);
                    stringBuilder.AppendLine(operationFailedItem.Exception.Message);
                    stringBuilder.Append(ExceptionCodeString);
                    stringBuilder.AppendLine(string.Format("0x{0:X8}", operationFailedItem.Exception.HResult));
                    return stringBuilder;
                });

                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制所有的错误内容到剪贴板
        /// </summary>
        private async void OnCopyOperationFailedClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                StringBuilder stringBuilder = await Task.Run(() =>
                {
                    StringBuilder stringBuilder = new();

                    foreach (OperationFailedModel operationFailedItem in OperationFailedCollection)
                    {
                        stringBuilder.Append(FileNameCopyString);
                        stringBuilder.AppendLine(operationFailedItem.FileName);
                        stringBuilder.Append(FilePathCopyString);
                        stringBuilder.AppendLine(operationFailedItem.FilePath);
                        stringBuilder.Append(ExceptionMessageString);
                        stringBuilder.AppendLine(operationFailedItem.Exception.Message);
                        stringBuilder.Append(ExceptionCodeString);
                        stringBuilder.AppendLine(string.Format("0x{0:X8}", operationFailedItem.Exception.HResult));
                        stringBuilder.AppendLine();
                    }

                    return stringBuilder;
                });

                copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(stringBuilder));
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
        }
    }
}
