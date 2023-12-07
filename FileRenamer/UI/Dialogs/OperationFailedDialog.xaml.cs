﻿using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Controls.Extensions;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using FileRenamer.Strings;
using FileRenamer.UI.TeachingTips;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 错误信息列表对话框
    /// </summary>
    public sealed partial class OperationFailedDialog : ContentDialog
    {
        private ObservableCollection<OperationFailedModel> OperationFailedList { get; } = new ObservableCollection<OperationFailedModel>();

        public OperationFailedDialog(ObservableCollection<OperationFailedModel> operationFailedList)
        {
            InitializeComponent();

            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedList.Add(operationFailedItem);
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
            foreach (OperationFailedModel operationFailedItem in OperationFailedList)
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
            TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.OperationFailed, true, OperationFailedList.Count));
        }
    }
}
