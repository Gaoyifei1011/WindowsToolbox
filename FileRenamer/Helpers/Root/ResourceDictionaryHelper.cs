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

        public static ResourceDictionary FlyoutResourceDict { get; private set; }

        public static ResourceDictionary GridResourceDict { get; private set; }

        public static ResourceDictionary HyperlinkButtonResourceDict { get; private set; }

        public static ResourceDictionary InAppNotificationResourceDict { get; private set; }

        public static ResourceDictionary ListViewResourceDict { get; private set; }

        public static ResourceDictionary MenuFlyoutResourceDict { get; private set; }

        public static ResourceDictionary NavigationViewResourceDict { get; private set; }

        public static ResourceDictionary ScrollViewerResourceDict { get; private set; }

        public static ResourceDictionary TextBlockResourceDict { get; private set; }

        /// <summary>
        /// 初始化资源字典信息
        /// </summary>
        public static void InitializeResourceDictionary()
        {
            ButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[1];
            ContentDialogResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[2];
            ExpanderResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[3];
            FlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[4];
            GridResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[5];
            HyperlinkButtonResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[6];
            InAppNotificationResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[7];
            ListViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[8];
            MenuFlyoutResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[9];
            NavigationViewResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[10];
            ScrollViewerResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[11];
            TextBlockResourceDict = Program.ApplicationRoot.Resources.MergedDictionaries[12];
        }
    }
}
