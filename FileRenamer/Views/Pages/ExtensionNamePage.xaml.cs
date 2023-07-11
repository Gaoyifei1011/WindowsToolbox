using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Services.Root;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 扩展名称页面
    /// </summary>
    public sealed partial class ExtensionNamePage : Page
    {
        public ExtensionNamePage()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(ExtensionNameSelectedType selectedType, ExtensionNameSelectedType comparedType)
        {
            return selectedType == comparedType;
        }

        public string LocalizeTotal(int count)
        {
            return string.Format(ResourceService.GetLocalized("ExtensionName/Total"), ViewModel.ExtensionNameDataList.Count);
        }
    }
}
