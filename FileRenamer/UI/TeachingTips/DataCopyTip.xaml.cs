using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Strings;
using Microsoft.UI.Xaml.Controls;

namespace FileRenamer.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyTip : TeachingTip
    {
        public DataCopyTip(DataCopyKind dataCopyKind, bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(dataCopyKind, isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(DataCopyKind dataCopyKind, bool isMultiSelected, int count)
        {
            switch (dataCopyKind)
            {
                case DataCopyKind.AppInformation:
                    {
                        Content = Notification.AppInformationCopy;
                        break;
                    }
                case DataCopyKind.OperationFailed:
                    {
                        if (isMultiSelected)
                        {
                            Content = string.Format(Notification.OperationFailedSelectedCopy, count);
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
