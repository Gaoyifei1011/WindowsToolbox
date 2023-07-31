using FileRenamer.Views.CustomControls.Notifications;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 列表内容为空时应用内通知
    /// </summary>
    public sealed partial class ListEmptyNotification : InAppNotification
    {
        public ListEmptyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}
