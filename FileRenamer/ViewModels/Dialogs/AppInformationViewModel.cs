using FileRenamer.Helpers.Root;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框视图模型
    /// </summary>
    public sealed class AppInformationViewModel : ViewModelBase
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
            //new AppInformationCopyNotification(true).Show();
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        public void InitializeAppInformation()
        {
            FileVersionInfo WindowsUIFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}\{1}", Environment.SystemDirectory, "Windows.UI.Xaml.dll"));
            WindowsUIVersion = WindowsUIFile.ProductVersion;

            FileVersionInfo MileXamlFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}{1}", InfoHelper.GetAppInstalledLocation(), @"Mile.Xaml.Managed.dll"));
            MileXamlVersion = MileXamlFile.FileVersion;

            // .NET 版本信息
            DoNetVersion = Convert.ToString(Environment.Version);
        }
    }
}
