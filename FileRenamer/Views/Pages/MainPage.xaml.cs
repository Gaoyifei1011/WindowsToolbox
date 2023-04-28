using FileRenamer.Services.Window;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current { get; private set; }

        public MainPage()
        {
            Current = this;
            InitializeComponent();
            NavigationService.NavigationFrame = MainFrame;
        }
    }
}
