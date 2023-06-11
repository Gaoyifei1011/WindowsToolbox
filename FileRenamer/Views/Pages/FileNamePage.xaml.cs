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

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("FileName/Total"), ViewModel.FileNameDataList.Count);
        }
    }
}
