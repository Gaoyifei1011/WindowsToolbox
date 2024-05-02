using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Strings;

namespace WindowsTools.UI.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip
    {
        public OperationResultTip(OperationKind operationKind, int successItems = 0, int failedItems = 0)
        {
            InitializeComponent();

            if (operationKind is OperationKind.File)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = failedItems is 0 ?
                    string.Format(Notification.FileResultSuccessfully, successItems) :
                    string.Format(Notification.FileResultFailed, successItems, failedItems);
            }
            else if (operationKind is OperationKind.IconExtract)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = failedItems is 0 ?
                    string.Format(Notification.IconExtractSuccessfully, successItems) :
                    string.Format(Notification.IconExtractFailed, successItems, failedItems);
            }
            else if (operationKind is OperationKind.GenerateTextEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.GenerateTextEmpty;
            }
            else if (operationKind is OperationKind.GenerateBarCodeFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.GenerateBarCodeFailed;
            }
            else if (operationKind is OperationKind.GenerateQRCodeFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.GenerateQRCodeFailed;
            }
            else if (operationKind is OperationKind.ParsePhotoFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.ParsePhotoFailed;
            }
            else if (operationKind is OperationKind.ReadClipboardImageFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.ReadClipboardImageFailed;
            }
            else if (operationKind is OperationKind.DeleteFileFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = Notification.FileDeleteFailed;
            }
        }
    }
}
