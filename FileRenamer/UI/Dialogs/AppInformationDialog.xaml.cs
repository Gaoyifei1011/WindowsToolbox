using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Helpers.Root;
using FileRenamer.Strings;
using FileRenamer.UI.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _windowsAppSDKVersion;

        public string WindowsAppSDKVersion
        {
            get { return _windowsAppSDKVersion; }

            set
            {
                _windowsAppSDKVersion = value;
                OnPropertyChanged();
            }
        }

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
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                if (RuntimeHelper.IsMSIX)
                {
                    IReadOnlyList<Package> DependencyList = Package.Current.Dependencies;

                    foreach (Package dependency in DependencyList)
                    {
                        if (dependency.DisplayName.Contains("WindowsAppRuntime"))
                        {
                            // Windows 应用 SDK 版本信息
                            Program.MainWindow.Invoke(() =>
                            {
                                WindowsAppSDKVersion = string.Format("{0}.{1}.{2}.{3}",
                                    dependency.Id.Version.Major,
                                    dependency.Id.Version.Minor,
                                    dependency.Id.Version.Build,
                                    dependency.Id.Version.Revision);
                            });
                        }
                    }
                }

                FileVersionInfo WindowsUIFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}\{1}", Environment.SystemDirectory, "Windows.UI.Xaml.dll"));

                FileVersionInfo MileXamlFile = FileVersionInfo.GetVersionInfo(Path.Combine(AppContext.BaseDirectory, @"Mile.Xaml.Managed.dll"));

                Program.MainWindow.Invoke(() =>
                {
                    WindowsUIVersion = string.Format("{0}.{1}.{2}.{3}",
                        WindowsUIFile.ProductMajorPart,
                        WindowsUIFile.ProductMinorPart,
                        WindowsUIFile.ProductBuildPart,
                        WindowsUIFile.ProductPrivatePart
                        );

                    MileXamlVersion = MileXamlFile.FileVersion;

                    // .NET 版本信息
                    DoNetVersion = Convert.ToString(RuntimeInformation.FrameworkDescription.Remove(0, 15));
                });
            });
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        public void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder stringBuilder = new StringBuilder();
            if (RuntimeHelper.IsMSIX)
            {
                stringBuilder.AppendLine(Dialog.WindowsAppSDKVersion + WindowsAppSDKVersion);
            }
            stringBuilder.AppendLine(Dialog.WindowsUIVersion + WindowsUIVersion);
            stringBuilder.AppendLine(Dialog.MileXamlVersion + MileXamlVersion);
            stringBuilder.AppendLine(Dialog.DoNetVersion + DoNetVersion);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            sender.Hide();
            new DataCopyNotification(this, DataCopyKind.AppInformation).Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
