using FileRenamer.Services.Window;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 应用主窗口页面
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

        public void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(XamlRoot);
            foreach (Popup popup in PopupRoot)
            {
                if (popup.Child as MenuFlyoutPresenter != null)
                {
                    popup.IsOpen = false;
                }

                if (popup.Child as Canvas != null)
                {
                    popup.IsOpen = false;
                }
            }
        }
    }
}
