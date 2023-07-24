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
            ExpanderResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[1];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];
            InAppNotificationResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[3];
            ListViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[4];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[5];
            ScrollBarResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[6];
        }
    }
}
