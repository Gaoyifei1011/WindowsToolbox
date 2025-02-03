using Windows.UI.Xaml;

namespace WindowsTools.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        /// <summary>
        /// 确定当前选择的索引是否为目标控件
        /// </summary>
        public static Visibility IsCurrentControl(int selectedIndex, int index)
        {
            return selectedIndex.Equals(index) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility IsCurrentItem(object selectedItem, object item)
        {
            return selectedItem.Equals(item) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
