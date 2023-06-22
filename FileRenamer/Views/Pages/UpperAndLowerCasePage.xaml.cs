using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 大写小写页面
    /// </summary>
    public sealed partial class UpperAndLowerCasePage : Page
    {
        public UpperAndLowerCasePage()
        {
            InitializeComponent();
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("UpperAndLowerCase/Total"), ViewModel.UpperAndLowerCaseDataList.Count);
        }
    }
}
