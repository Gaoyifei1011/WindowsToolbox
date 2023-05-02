using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace FileRenamer.Views.CustomControls.Layout
{
    /// <summary>
    /// 自定义的扩展器控件
    /// </summary>
    public partial class Expander : ContentControl
    {
        public object Header
        {
            get { return GetValue(HeaderProperty); }

            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(Expander), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }

            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Expander), new PropertyMetadata(null));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander), new PropertyMetadata(null, OnIsExpandedPropertyChanged));

        public double NegativeContentHeight
        {
            get { return (double)GetValue(NegativeContentHeightProperty); }
            set { SetValue(NegativeContentHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NegativeContentHeightProperty =
            DependencyProperty.Register("NegativeContentHeight", typeof(double), typeof(Expander), new PropertyMetadata(0.0));

        public Expander()
        { }

        protected override void OnApplyTemplate()
        {
            Border expanderContentClip = (Border)GetTemplateChild("ExpanderContentClip");
            if (expanderContentClip is not null)
            {
                Visual visual = ElementCompositionPreview.GetElementVisual(expanderContentClip);
                visual.Clip = visual.Compositor.CreateInsetClip();
            }
            Border expanderContent = (Border)GetTemplateChild("ExpanderContent");
            if (expanderContent is not null)
            {
                expanderContent.SizeChanged += OnContentSizeChanged;
            }

            UpdateExpandState(false);
        }

        public void UpdateExpandState(bool useTransitions)
        {
            bool isExpanded = IsExpanded;

            if (isExpanded)
            {
                VisualStateManager.GoToState(this, "ExpandDown", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "CollapseUp", useTransitions);
            }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Expander owner = d as Expander;
            owner.UpdateExpandState(true);
        }

        private void OnContentSizeChanged(object sender, SizeChangedEventArgs args)
        {
            double height = args.NewSize.Height;
            NegativeContentHeight = -1.0 * height;
        }
    }
}
