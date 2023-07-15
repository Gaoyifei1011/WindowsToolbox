using FileRenamer.Helpers.Root;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ExtendedContentDialog, INotifyPropertyChanged
    {
        private string _windowsUIVersion;

        public string WindowsUIVersion
        {
            get { return _windowsUIVersion; }

            set
            {
                _windowsUIVersion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsUIVersion)));
            }
        }

        private string _mileXamlVersion;

        public string MileXamlVersion
        {
            get { return _mileXamlVersion; }

            set
            {
                _mileXamlVersion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MileXamlVersion)));
            }
        }

        private string _doNetVersion;

        public string DoNetVersion
        {
            get { return _doNetVersion; }

            set
            {
                _doNetVersion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoNetVersion)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();
            FileVersionInfo WindowsUIFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}\{1}", Environment.SystemDirectory, "Windows.UI.Xaml.dll"));
            WindowsUIVersion = string.Format("{0}.{1}.{2}.{3}",
                WindowsUIFile.ProductMajorPart,
                WindowsUIFile.ProductMinorPart,
                WindowsUIFile.ProductBuildPart,
                WindowsUIFile.ProductPrivatePart
                );

            FileVersionInfo MileXamlFile = FileVersionInfo.GetVersionInfo(Path.Combine(AppContext.BaseDirectory, @"Mile.Xaml.Managed.dll"));
            MileXamlVersion = MileXamlFile.FileVersion;

            // .NET 版本信息
            DoNetVersion = Convert.ToString(Environment.Version);
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        public void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WindowsUIVersion") + WindowsUIVersion);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/MileXamlVersion") + MileXamlVersion);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/DoNetVersion") + DoNetVersion);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            sender.Hide();
            new AppInformationCopyNotification().Show();
        }
    }
}
