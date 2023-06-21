using FileRenamer.Views.CustomControls.DialogsAndFlyouts;

namespace FileRenamer.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框视图
    /// </summary>
    public sealed partial class AppInformationDialog : ExtendedContentDialog
    {
        public AppInformationDialog()
        {
            InitializeComponent();
            ViewModel.InitializeAppInformation();
        }
    }
}
