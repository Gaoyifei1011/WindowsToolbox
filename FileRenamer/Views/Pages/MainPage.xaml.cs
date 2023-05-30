using FileRenamer.Services.Window;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
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
            //InitializeFrostedGlass(GlassHost);
        }

        private void InitializeFrostedGlass(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = backdropBrush;
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);
            glassVisual.StartAnimation("Size", bindSizeAnimation);
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
