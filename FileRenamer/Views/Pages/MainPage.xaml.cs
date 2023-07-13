using FileRenamer.Services.Window;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 应用主窗口页面视图
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = MainFrame;
        }

        /// <summary>
        /// 检查当前页面是否为目标页面
        /// </summary>
        public bool IsCurrentPage(object selectedItem, string pageName)
        {
            return selectedItem.ToString() == pageName;
        }
    }
}
