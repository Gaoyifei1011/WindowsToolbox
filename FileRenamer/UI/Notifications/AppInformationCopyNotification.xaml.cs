using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 应用信息复制应用内通知
    /// </summary>
    public sealed partial class AppInformationCopyNotification : InAppNotification
    {
        public AppInformationCopyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}
