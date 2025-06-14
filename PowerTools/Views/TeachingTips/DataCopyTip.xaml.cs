using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;

namespace PowerTools.Views.TeachingTips
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
                            Content = ResourceService.NotificationResource.GetString("AppInformationCopy");
                            break;
                        }
                    case DataCopyKind.FilePath:
                        {
                            Content = isMultiSelected ? string.Format(ResourceService.NotificationResource.GetString("FilePathSelectedCopy"), count) : (object)ResourceService.NotificationResource.GetString("FilePathCopy");
                            break;
                        }
                    case DataCopyKind.OperationFailed:
                        {
                            Content = isMultiSelected ? string.Format(ResourceService.NotificationResource.GetString("OperationFailedSelectedCopy"), count) : (object)ResourceService.NotificationResource.GetString("OperationFailedCopy");
                            break;
                        }
                    case DataCopyKind.String:
                        {
                            Content = isMultiSelected ? string.Format(ResourceService.NotificationResource.GetString("StringSelectedCopy"), count) : (object)ResourceService.NotificationResource.GetString("StringCopy");
                            break;
                        }
                    case DataCopyKind.UpdateInformation:
                        {
                            Content = ResourceService.NotificationResource.GetString("UpdateInformationCopy");
                            break;
                        }
                }
            }
            else
            {
                CopySuccess.Visibility = Visibility.Collapsed;
                CopyFailed.Visibility = Visibility.Visible;
                CopyFailed.Text = ResourceService.NotificationResource.GetString("CopyToClipboardFailed");
            }
        }
    }
}
