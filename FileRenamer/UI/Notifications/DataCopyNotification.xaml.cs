using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Strings;
using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyNotification : InAppNotification
    {
        private bool IsMultiSelected;
        private int Count;
        private DataCopyKind DataCopyType;

        public DataCopyNotification(FrameworkElement element, DataCopyKind copyType, bool isMultiSelected = false, int count = 0) : base(element)
        {
            InitializeComponent();
            DataCopyType = copyType;
            IsMultiSelected = isMultiSelected;
            Count = count;

            InitializeContent();
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent()
        {
            switch (DataCopyType)
            {
                case DataCopyKind.AppInformation:
                    {
                        Content = Notification.AppInformationCopy;
                        break;
                    }
                case DataCopyKind.OperationFailed:
                    {
                        if (IsMultiSelected)
                        {
                            Content = string.Format(Notification.OperationFailedSelectedCopy, Count);
                        }
                        else
                        {
                            Content = Notification.OperationFailedCopy;
                        }
                        break;
                    }
            }
        }
    }
}
