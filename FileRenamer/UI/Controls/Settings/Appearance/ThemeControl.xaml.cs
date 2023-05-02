using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：应用主题设置用户控件视图
    /// </summary>
    public sealed partial class ThemeControl : UserControl
    {
        public ThemeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
