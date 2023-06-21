using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 屏幕缩放通知对话框视图
    /// </summary>
    public sealed partial class NameChangeDialog : Grid
    {
        public NameChangeDialog()
        {
            InitializeComponent();
        }

        public string GetChangeRule(int index)
        {
            return string.Format(ResourceService.GetLocalized("Dialog/ChangeRule"), ViewModel.NameChangeRuleList[index]);
        }
    }
}
