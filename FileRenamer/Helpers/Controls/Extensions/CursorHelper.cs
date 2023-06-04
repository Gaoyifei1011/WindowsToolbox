using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace FileRenamer.Helpers.Controls.Extensions
{
    /// <summary>
    /// 附加属性：设置指针位于控件上时显示的游标
    /// </summary>
    public static class CursorHelper
    {
        private static readonly object _cursorLock = new object();
        private static readonly CoreCursor _defaultCursor = new CoreCursor(CoreCursorType.Arrow, 1);

        private static readonly Dictionary<CoreCursorType, CoreCursor> _cursors =
            new Dictionary<CoreCursorType, CoreCursor> { { CoreCursorType.Arrow, _defaultCursor } };

        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached("Cursor", typeof(CoreCursorType), typeof(CursorHelper), new PropertyMetadata(CoreCursorType.Arrow, CursorChanged));

        public static void SetCursor(FrameworkElement element, CoreCursorType value)
        {
            element.SetValue(CursorProperty, value);
        }

        public static CoreCursorType GetCursor(FrameworkElement element)
        {
            return (CoreCursorType)element.GetValue(CursorProperty);
        }

        private static void CursorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null)
            {
                throw new NullReferenceException(nameof(element));
            }

            var value = (CoreCursorType)e.NewValue;

            lock (_cursorLock)
            {
                if (!_cursors.ContainsKey(value))
                {
                    _cursors[value] = new CoreCursor(value, 1);
                }

                element.PointerEntered -= Element_PointerEntered;
                element.PointerEntered += Element_PointerEntered;
                element.PointerExited -= Element_PointerExited;
                element.PointerExited += Element_PointerExited;
                element.Unloaded -= ElementOnUnloaded;
                element.Unloaded += ElementOnUnloaded;
            }
        }

        private static void Element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CoreCursorType cursor = GetCursor((FrameworkElement)sender);
            Window.Current.CoreWindow.PointerCursor = _cursors[cursor];
        }

        private static void Element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CoreCursor cursor;
            if (sender != e.OriginalSource && e.OriginalSource is FrameworkElement newElement)
            {
                cursor = _cursors[GetCursor(newElement)];
            }
            else
            {
                cursor = _defaultCursor;
            }

            Window.Current.CoreWindow.PointerCursor = cursor;
        }

        private static void ElementOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Window.Current.CoreWindow.PointerCursor = _defaultCursor;
        }
    }
}
