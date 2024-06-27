using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Strings;

namespace WindowsTools.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyTip : TeachingTip
    {
        public DataCopyTip(DataCopyKind dataCopyKind, bool isSuccessfully, bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(dataCopyKind, isSuccessfully, isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(DataCopyKind dataCopyKind, bool isSuccessfully, bool isMultiSelected, int count)
        {
            if (isSuccessfully)
            {
                CopySuccess.Visibility = Visibility.Visible;
                CopyFailed.Visibility = Visibility.Collapsed;

                switch (dataCopyKind)
                {
                    case DataCopyKind.AppInformation:
                        {
                            Content = Notification.AppInformationCopy;
                            break;
                        }
                    case DataCopyKind.FilePath:
                        {
                            Content = isMultiSelected ? string.Format(Notification.FilePathSelectedCopy, count) : (object)Notification.FilePathCopy;
                            break;
                        }
                    case DataCopyKind.OperationFailed:
                        {
                            Content = isMultiSelected ? string.Format(Notification.OperationFailedSelectedCopy, count) : (object)Notification.OperationFailedCopy;
                            break;
                        }
                    case DataCopyKind.String:
                        {
                            Content = isMultiSelected ? string.Format(Notification.StringSelectedCopy, count) : (object)Notification.StringCopy;
                            break;
                        }
                    case DataCopyKind.UpdateInformation:
                        {
                            Content = Notification.UpdateInformationCopy;
                            break;
                        }
                }
            }
            else
            {
                CopySuccess.Visibility = Visibility.Collapsed;
                CopyFailed.Visibility = Visibility.Visible;
                CopyFailed.Text = Notification.CopyToClipboardFailed;
            }
        }
    }
}
