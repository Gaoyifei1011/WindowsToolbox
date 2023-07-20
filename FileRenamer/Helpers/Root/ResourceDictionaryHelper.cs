using Windows.UI.Xaml;

namespace FileRenamer.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ButtonResourceDict { get; private set; }

        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary ScrollViewerResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[1];
            ExpanderResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];
            GridResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[3];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[4];
            InAppNotificationResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[5];
            ListViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[6];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[7];
            ScrollViewerResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[8];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[9];
        }
    }
}
