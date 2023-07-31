using Windows.UI.Xaml;

namespace FileRenamer.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary ScrollBarResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ExpanderResourceDict = Application.Current.Resources.MergedDictionaries[1];
            HyperlinkButtonResourceDict = Application.Current.Resources.MergedDictionaries[2];
            InAppNotificationResourceDict = Application.Current.Resources.MergedDictionaries[3];
            ListViewResourceDict = Application.Current.Resources.MergedDictionaries[4];
            MenuFlyoutResourceDict = Application.Current.Resources.MergedDictionaries[5];
            ScrollBarResourceDict = Application.Current.Resources.MergedDictionaries[6];
        }
    }
}
