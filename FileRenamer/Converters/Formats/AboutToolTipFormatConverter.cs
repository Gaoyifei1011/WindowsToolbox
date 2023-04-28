using FileRenamer.Services.Root;
using System;
using Windows.UI.Xaml.Data;

namespace FileRenamer.Converters.Formats
{
    /// <summary>
    /// 关于界面项目引用和感谢介绍文字提示转换器
    /// </summary>
    public class AboutToolTipFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null || parameter is null)
            {
                return string.Empty;
            }

            string param = System.Convert.ToString(parameter);

            if (param is "Reference")
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("About/ReferenceToolTip"));
            }
            else if (param is "Thanks")
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("About/ThanksToolTip"));
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
