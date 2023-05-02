using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            AboutScroll.ViewChanged += OnViewChanged;
        }

        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            AboutScroll.ViewChanged -= OnViewChanged;
        }

        public void OnItemTapped(object sender, TappedRoutedEventArgs args)
        {
            double CurrentScrollPosition = AboutScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            ListViewItem item = sender as ListViewItem;

            if (Convert.ToString(item.Tag) == ViewModel.TagList[0])
            {
                Point TargetPosition = Introduction.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[1])
            {
                Point TargetPosition = Reference.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[2])
            {
                Point TargetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[3])
            {
                Point TargetPosition = Precaution.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[4])
            {
                Point TargetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
            else if (Convert.ToString(item.Tag) == ViewModel.TagList[5])
            {
                Point TargetPosition = Thanks.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                AboutScroll.ChangeView(null, TargetPosition.Y, null);
            }
        }

        public void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs args)
        {
            double CurrentScrollPosition = AboutScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            Point IntroductionPosition = Introduction.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));
            Point ReferencePosition = Reference.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));
            Point InstructionsPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));
            Point PrecautionPosition = Precaution.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));
            Point SettingsHelpPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));
            Point ThanksPosition = Thanks.TransformToVisual(AboutScroll).TransformPoint(new Point(0, 0));

            double[] controlBottomPosition =
            {
                IntroductionPosition.Y + Introduction.ActualHeight,
                ReferencePosition.Y + Reference.ActualHeight,
                InstructionsPosition.Y + Instructions.ActualHeight,
                PrecautionPosition.Y + Precaution.ActualHeight,
                SettingsHelpPosition.Y + SettingsHelp.ActualHeight,
                ThanksPosition.Y + Thanks.ActualHeight,
            };

            double minPosition = controlBottomPosition.Where(n => n > 0).Min(Math.Abs);
            int minPositionControlIndex = Array.IndexOf(controlBottomPosition, minPosition);
            ViewModel.SelectedIndex = minPositionControlIndex;
        }
    }
}
