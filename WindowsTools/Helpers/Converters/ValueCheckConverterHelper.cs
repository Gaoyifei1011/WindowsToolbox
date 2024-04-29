using Windows.UI.Xaml;
using WindowsTools.WindowsAPI.ComTypes;

namespace WindowsTools.Helpers.Converters
{
    public static class ValueCheckConverterHelper
    {
        public static Visibility DownloadStateCheck(DODownloadState value, DODownloadState checkValue)
        {
            return value == checkValue ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
