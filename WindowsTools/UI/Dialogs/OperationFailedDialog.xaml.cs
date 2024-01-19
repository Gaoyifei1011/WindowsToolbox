using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls.Extensions;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 错误信息列表对话框
    /// </summary>
    public sealed partial class OperationFailedDialog : ContentDialog
    {
        private ObservableCollection<OperationFailedModel> OperationFailedCollection { get; } = new ObservableCollection<OperationFailedModel>();

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
        private void OnCopyExecuteRequested(XamlUICommand command, ExecuteRequestedEventArgs args)
        {
            OperationFailedModel operationFailedItem = args.Parameter as OperationFailedModel;
            if (operationFailedItem is not null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Dialog.FileNameCopy);
                builder.AppendLine(operationFailedItem.FileName);
                builder.Append(Dialog.FilePathCopy);
                builder.AppendLine(operationFailedItem.FilePath);
                builder.Append(Dialog.ExceptionMessage);
                builder.AppendLine(operationFailedItem.Exception.Message);
                builder.Append(Dialog.ExceptionCode);
                builder.AppendLine(operationFailedItem.Exception.HResult.ToString());
                CopyPasteHelper.CopyToClipBoard(builder.ToString());
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.OperationFailed, false));
            }
        }

        /// <summary>
        /// 复制所有的错误内容到剪贴板
        /// </summary>
        private void OnCopyOperationFailedClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder builder = new StringBuilder();
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
            CopyPasteHelper.CopyToClipBoard(builder.ToString());
            sender.Hide();
            TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.OperationFailed, true, OperationFailedCollection.Count));
        }
    }
}
