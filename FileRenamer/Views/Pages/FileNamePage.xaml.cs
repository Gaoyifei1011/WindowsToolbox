using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 文件名称页面
    /// </summary>
    public sealed partial class FileNamePage : Page
    {
        public FileNamePage()
        {
            InitializeComponent();
        }

        public string GetChangeRule(int index)
        {
            return string.Format(ResourceService.GetLocalized("Dialog/ChangeRule"), ViewModel.NameChangeRuleList[index]);
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("FileName/Total"), ViewModel.FileNameDataList.Count);
        }
    }
}
