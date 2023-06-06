using FileRenamer.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// 语言设置修改成功时应用内通知视图
    /// </summary>
    public sealed partial class LanguageChangeNotification : InAppNotification
    {
        public LanguageChangeNotification([Optional, DefaultParameterValue(false)] bool setResult)
        {
            InitializeComponent();
            XamlRoot = Program.MainWindow.MainPage.XamlRoot;
            ViewModel.Initialize(setResult);
        }
    }
}
