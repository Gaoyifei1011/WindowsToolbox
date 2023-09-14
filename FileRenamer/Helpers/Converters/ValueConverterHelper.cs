using Windows.UI.Xaml;

namespace FileRenamer.Helpers.Converters
{
    /// <summary>
    /// 值类型 / 内容转换辅助类
    /// </summary>
    public static class ValueConverterHelper
    {
        /// <summary>
        /// 布尔值取反
        /// </summary>
        public static bool BooleanReverseConvert(bool value)
        {
            return !value;
        }

        public static bool StringCompareReverseConvert(string value, string comparedValue)
        {
            return !value.Equals(comparedValue);
        }

        /// <summary>
        /// 整数值与控件显示值转换
        /// </summary>
        public static Visibility IntToVisibilityConvert(int value)
        {
            return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 整数值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility IntToVisibilityReverseConvert(int value)
        {
            return value is 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 布尔值与控件显示值转换
        /// </summary>
        public static Visibility BooleanToVisibilityConvert(bool value)
        {
            return value is true ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility StringToVisibilityConvert(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }
    }
}
