using System;
using System.Collections.Concurrent;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.Views.CustomControls.DialogsAndFlyouts
{
    /// <summary>
    /// 与其组中其他单选菜单项互斥的菜单项控件
    /// </summary>
    public partial class RadioMenuFlyoutItem : ToggleMenuFlyoutItem
    {
        [ThreadStatic]
        private static ConcurrentDictionary<string, WeakReference<RadioMenuFlyoutItem>> s_selectionMap;

        // Copies of IsChecked & GroupName to avoid using those dependency properties in the ~RadioMenuFlyoutItem() destructor which would lead to crashes.
        private bool m_isChecked;

        private string m_groupName;

        private bool m_isSafeUncheck;

        public static bool GetAreCheckStatesEnabled(DependencyObject obj) => (bool)obj.GetValue(AreCheckStatesEnabledProperty);

        public static void SetAreCheckStatesEnabled(DependencyObject obj, bool value) => obj.SetValue(AreCheckStatesEnabledProperty, value);

        public static readonly DependencyProperty AreCheckStatesEnabledProperty =
            DependencyProperty.RegisterAttached("AreCheckStatesEnabled", typeof(bool), typeof(RadioMenuFlyoutItem), new PropertyMetadata(false, OnAreCheckStatesEnabledPropertyChanged));

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(RadioMenuFlyoutItem), new PropertyMetadata(string.Empty, (s, e) => (s as RadioMenuFlyoutItem)?.OnPropertyChanged(e)));

        public new bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public new static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(RadioMenuFlyoutItem), new PropertyMetadata(false, (s, e) => (s as RadioMenuFlyoutItem)?.OnPropertyChanged(e)));

        public RadioMenuFlyoutItem()
        {
            RegisterPropertyChangedCallback(ToggleMenuFlyoutItem.IsCheckedProperty, OnInternalIsCheckedChanged);

            // Ensure that this object exists
            s_selectionMap ??= new ConcurrentDictionary<string, WeakReference<RadioMenuFlyoutItem>>();

            DefaultStyleKey = typeof(RadioMenuFlyoutItem);
        }

        ~RadioMenuFlyoutItem()
        {
            // If this is the checked item, remove it from the lookup.
            if (m_isChecked)
            {
                if (m_groupName != null)
                {
                    s_selectionMap?.TryRemove(m_groupName, out _);
                }
            }
        }

        private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            DependencyProperty property = args.Property;
            if (property == IsCheckedProperty)
            {
                if (base.IsChecked != IsChecked)
                {
                    m_isSafeUncheck = true;
                    base.IsChecked = IsChecked;
                    m_isSafeUncheck = false;
                    UpdateCheckedItemInGroup();
                }
                m_isChecked = IsChecked;
            }
            else if (property == GroupNameProperty)
            {
                m_groupName = GroupName;
            }
        }

        private void OnInternalIsCheckedChanged(DependencyObject sender, DependencyProperty args)
        {
            if (!base.IsChecked)
            {
                if (m_isSafeUncheck)
                {
                    // The uncheck is due to another radio button being checked -- that's all right.
                    IsChecked = false;
                }
                else
                {
                    // The uncheck is due to user interaction -- not allowed.
                    base.IsChecked = true;
                }
            }
            else if (!IsChecked)
            {
                IsChecked = true;
                UpdateCheckedItemInGroup();
            }
        }

        private void UpdateCheckedItemInGroup()
        {
            if (IsChecked)
            {
                var groupName = GroupName;
                if (s_selectionMap.TryGetValue(groupName, out var previousCheckedItemWeak))
                {
                    if (previousCheckedItemWeak.TryGetTarget(out var previousCheckedItem))
                    {
                        // Uncheck the previously checked item.
                        previousCheckedItem.IsChecked = false;
                    }
                }

                s_selectionMap[groupName] = new WeakReference<RadioMenuFlyoutItem>(this);
            }
        }

        private static void OnAreCheckStatesEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is true)
            {
                if (sender is MenuFlyoutSubItem subMenu)
                {
                    // Every time the submenu is loaded, see if it contains a checked RadioMenuFlyoutItem or not.
                    subMenu.Loaded += (object sender, RoutedEventArgs _) =>
                    {
                        bool isAnyItemChecked = false;
                        foreach (var item in subMenu.Items)
                        {
                            if (item is RadioMenuFlyoutItem radioItem)
                            {
                                isAnyItemChecked = isAnyItemChecked || radioItem.IsChecked;
                            }
                        }
                        VisualStateManager.GoToState(subMenu, isAnyItemChecked ? "Checked" : "Unchecked", false);
                    };
                }
            }
        }
    }
}
