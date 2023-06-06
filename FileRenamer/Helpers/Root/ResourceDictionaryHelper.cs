using Windows.UI.Xaml;

namespace FileRenamer.Helpers.Root
{
    /// <summary>
    /// 应用资源字典
    /// </summary>
    public static class ResourceDictionaryHelper
    {
        public static ResourceDictionary ButtonResourceDict { get; private set; }

        public static ResourceDictionary ContentDialogResourceDict { get; private set; }

        public static ResourceDictionary ExpanderResourceDict { get; private set; }

        public static ResourceDictionary FontIconResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary GridViewResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary NavigationViewResourceDict { get; private set; }

        public static ResourceDictionary ScrollViewerResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];
            ContentDialogResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[3];
            ExpanderResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[4];
            FontIconResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[5];
            GridResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[6];
            GridViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[7];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[8];
            InAppNotificationResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[9];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[10];
            NavigationViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[11];
            ScrollViewerResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[12];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[13];
        }
    }
}
