using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 错误信息列表对话框
    /// </summary>
    public sealed partial class OperationFailedDialog : ContentDialog
    {
        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = [];

        public OperationFailedDialog(ObservableCollection<OperationFailedModel> operationFailedCollection)
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
                StringBuilder builder = new();
                builder.Append(Dialog.FileNameCopy);
                builder.AppendLine(operationFailedItem.FileName);
                builder.Append(Dialog.FilePathCopy);
                builder.AppendLine(operationFailedItem.FilePath);
                builder.Append(Dialog.ExceptionMessage);
                builder.AppendLine(operationFailedItem.Exception.Message);
                builder.Append(Dialog.ExceptionCode);
                builder.AppendLine(operationFailedItem.Exception.HResult.ToString());
                bool copyResult = CopyPasteHelper.CopyToClipboard(builder.ToString());
                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.OperationFailed, copyResult, false));
            }
        }

        /// <summary>
        /// 复制所有的错误内容到剪贴板
        /// </summary>
        private async void OnCopyOperationFailedClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder builder = new();
            foreach (OperationFailedModel operationFailedItem in OperationFailedCollection)
            {
                builder.Append(Dialog.FileNameCopy);
                builder.AppendLine(operationFailedItem.FileName);
                builder.Append(Dialog.FilePathCopy);
                builder.AppendLine(operationFailedItem.FilePath);
                builder.Append(Dialog.ExceptionMessage);
                builder.AppendLine(operationFailedItem.Exception.Message);
                builder.Append(Dialog.ExceptionCode);
                builder.AppendLine(operationFailedItem.Exception.HResult.ToString());
                builder.AppendLine();
            }
            bool copyResult = CopyPasteHelper.CopyToClipboard(builder.ToString());
            sender.Hide();
            await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.OperationFailed, copyResult, true, OperationFailedCollection.Count));
        }
    }
}
