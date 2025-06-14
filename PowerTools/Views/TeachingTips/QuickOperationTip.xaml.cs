using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;

namespace PowerTools.Views.TeachingTips
{
    /// <summary>
    /// 快捷操作应用内通知
    /// </summary>
    public sealed partial class QuickOperationTip : TeachingTip
    {
        public QuickOperationTip(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully = false)
        {
            InitializeComponent();
            InitializeContent(quickOperationKind, isPinnedSuccessfully);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully)
        {
            QuickOperationSuccess.Visibility = isPinnedSuccessfully ? Visibility.Visible : Visibility.Collapsed;
            QuickOperationFailed.Visibility = isPinnedSuccessfully ? Visibility.Collapsed : Visibility.Visible;
            if (quickOperationKind is QuickOperationKind.Desktop && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.NotificationResource.GetString("DesktopShortcutSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.Desktop && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.NotificationResource.GetString("DesktopShortFailed");
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.NotificationResource.GetString("StartScreenSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.NotificationResource.GetString("StartScreenFailed");
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.NotificationResource.GetString("TaskbarSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.NotificationResource.GetString("TaskbarFailed");
            }
        }
    }
}
