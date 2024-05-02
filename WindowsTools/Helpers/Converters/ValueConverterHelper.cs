using System;
using Windows.UI.Xaml;

namespace WindowsTools.Helpers.Converters
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

        /// <summary>
        /// 字符串布尔值转换
        /// </summary>
        public static bool StringToBooleanConvert(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool ObjectCompareReverseConvert(object value, object comparedValue)
        {
            return !value.Equals(comparedValue);
        }

        public static Uri UriConvert(object uri)
        {
            return new Uri(uri.ToString());
        }

        /// <summary>
        /// 整数值与布尔值转换
        /// </summary>
        public static bool IntToBooleanConvert(int value)
        {
            return value is not 0;
        }

        /// <summary>
        /// 布尔值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility BooleanToVisibilityReverseConvert(bool value)
        {
            return value is false ? Visibility.Visible : Visibility.Collapsed;
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
        /// 字符串与控件显示值转换
        /// </summary>
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

        /// <summary>
        /// 对象值与控件显示值转换
        /// </summary>
        public static Visibility ObjectToVisibilityConvert(object value, object comparedValue)
        {
            return value.Equals(comparedValue) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
