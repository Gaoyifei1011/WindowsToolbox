using FileRenamer.Helpers.Root;
using FileRenamer.Services.Root;
using FileRenamer.UI.Notifications;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _windowsUIVersion;

        public string WindowsUIVersion
        {
            get { return _windowsUIVersion; }

            set
            {
                _windowsUIVersion = value;
                OnPropertyChanged();
            }
        }

        private string _mileXamlVersion;

        public string MileXamlVersion
        {
            get { return _mileXamlVersion; }

            set
            {
                _mileXamlVersion = value;
                OnPropertyChanged();
            }
        }

        private string _doNetVersion;

        public string DoNetVersion
        {
            get { return _doNetVersion; }

            set
            {
                _doNetVersion = value;
                OnPropertyChanged();
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
            DoNetVersion = Convert.ToString(RuntimeInformation.FrameworkDescription.Remove(0, 15));
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
            new AppInformationCopyNotification(this).Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
