using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 文件属性页面
    /// </summary>
    public sealed partial class FilePropertiesPage : Page
    {
        public FilePropertiesPage()
        {
            InitializeComponent();
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("FileProperties/Total"), ViewModel.FilePropertiesDataList.Count);
        }
    }
}
