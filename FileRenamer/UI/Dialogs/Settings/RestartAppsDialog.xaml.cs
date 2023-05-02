using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System;
using Windows.UI.Xaml;

namespace FileRenamer.UI.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图
    /// </summary>
    public sealed partial class RestartAppsDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public RestartAppsDialog()
        {
            InitializeComponent();
        }
    }
}
