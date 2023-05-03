using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            SettingsScroll.ViewChanged += OnViewChanged;
        }

        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            SettingsScroll.ViewChanged -= OnViewChanged;
        }

        public void OnItemTapped(object sender, TappedRoutedEventArgs args)
        {
            double CurrentScrollPosition = SettingsScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            ListViewItem item = sender as ListViewItem;

            if (Convert.ToString(item.Tag) == ViewModel.TagList[0])
            {
                Point TargetPosition = Appearance.TransformToVisual(SettingsScroll).TransformPoint(CurrentPoint);
                SettingsScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[1])
            {
                Point TargetPosition = General.TransformToVisual(SettingsScroll).TransformPoint(CurrentPoint);
                SettingsScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[2])
            {
                Point TargetPosition = Advanced.TransformToVisual(SettingsScroll).TransformPoint(CurrentPoint);
                SettingsScroll.ChangeView(null, TargetPosition.Y, null);
            }
        }

        public void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs args)
        {
            double CurrentScrollPosition = SettingsScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            Point AppearancePosition = Appearance.TransformToVisual(SettingsScroll).TransformPoint(new Point(0, 0));
            Point GeneralPosition = General.TransformToVisual(SettingsScroll).TransformPoint(new Point(0, 0));
            Point AdvancedPosition = Advanced.TransformToVisual(SettingsScroll).TransformPoint(new Point(0, 0));

            double[] controlBottomPosition =
            {
                AppearancePosition.Y + Appearance.ActualHeight,
                GeneralPosition.Y + General.ActualHeight,
                AdvancedPosition.Y + Advanced.ActualHeight,
            };

            double minPosition = controlBottomPosition.Where(n => n > 0).Min(Math.Abs);
            int minPositionControlIndex = Array.IndexOf(controlBottomPosition, minPosition);
            ViewModel.SelectedIndex = minPositionControlIndex;
        }
    }
}
