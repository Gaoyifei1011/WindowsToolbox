using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.Strings;
using WindowsTools.UI.TeachingTips;

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        private string _windowsAppSDKVersion;

        public string WindowsAppSDKVersion
        {
            get { return _windowsAppSDKVersion; }

            set
            {
                if (!Equals(_windowsAppSDKVersion, value))
                {
                    _windowsAppSDKVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsAppSDKVersion)));
                }
            }
        }

        private string _winUI2Version;

        public string WinUI2Version
        {
            get { return _winUI2Version; }

            set
            {
                if (!Equals(_winUI2Version, value))
                {
                    _winUI2Version = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinUI2Version)));
                }
            }
        }

        private string _windowsUIVersion;

        public string WindowsUIVersion
        {
            get { return _windowsUIVersion; }

            set
            {
                if (!Equals(_windowsUIVersion, value))
                {
                    _windowsUIVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsUIVersion)));
                }
            }
        }

        private string _mileXamlVersion;

        public string MileXamlVersion
        {
            get { return _mileXamlVersion; }

            set
            {
                if (!Equals(_mileXamlVersion, value))
                {
                    _mileXamlVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MileXamlVersion)));
                }
            }
        }

        private string _doNetVersion;

        public string DoNetVersion
        {
            get { return _doNetVersion; }

            set
            {
                if (!Equals(_doNetVersion, value))
                {
                    _doNetVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoNetVersion)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                foreach (Package dependency in Package.Current.Dependencies)
                {
                    if (dependency.DisplayName.Contains("WindowsAppRuntime"))
                    {
                        // Windows 应用 SDK 版本信息
                        synchronizationContext.Post(_ =>
                        {
                            WindowsAppSDKVersion = string.Format("{0}.{1}.{2}.{3}",
                                dependency.Id.Version.Major,
                                dependency.Id.Version.Minor,
                                dependency.Id.Version.Build,
                                dependency.Id.Version.Revision);
                        }, null);
                    }

                    // WinUI 2 版本信息
                    if (dependency.DisplayName.Contains("Microsoft.UI.Xaml.2.8"))
                    {
                        FileVersionInfo winUI2File = FileVersionInfo.GetVersionInfo(Path.Combine(dependency.InstalledPath, "Microsoft.UI.Xaml.dll"));

                        synchronizationContext.Post(_ =>
                        {
                            WinUI2Version = string.Format("{0}.{1}.{2}.{3}",
                                winUI2File.ProductMajorPart,
                                winUI2File.ProductMinorPart,
                                winUI2File.ProductBuildPart,
                                winUI2File.ProductPrivatePart);
                        }, null);
                    }
                }

                // Windows UI 版本信息
                FileVersionInfo windowsUIFile = FileVersionInfo.GetVersionInfo(string.Format(@"{0}\{1}", Environment.SystemDirectory, "Windows.UI.Xaml.dll"));

                FileVersionInfo mileXamlFile = FileVersionInfo.GetVersionInfo(Path.Combine(AppContext.BaseDirectory, @"Mile.Xaml.Managed.dll"));

                synchronizationContext.Post(_ =>
                {
                    WindowsUIVersion = string.Format("{0}.{1}.{2}.{3}",
                        windowsUIFile.ProductMajorPart,
                        windowsUIFile.ProductMinorPart,
                        windowsUIFile.ProductBuildPart,
                        windowsUIFile.ProductPrivatePart
                        );

                    MileXamlVersion = mileXamlFile.FileVersion;

                    // .NET 版本信息
                    DoNetVersion = Convert.ToString(RuntimeInformation.FrameworkDescription.Remove(0, 15));
                }, null);
            });
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(Dialog.WindowsAppSDKVersion + WindowsAppSDKVersion);
            stringBuilder.AppendLine(Dialog.WinUI2Version + WinUI2Version);
            stringBuilder.AppendLine(Dialog.WindowsUIVersion + WindowsUIVersion);
            stringBuilder.AppendLine(Dialog.MileXamlVersion + MileXamlVersion);
            stringBuilder.AppendLine(Dialog.DoNetVersion + DoNetVersion);

            bool copyResult = CopyPasteHelper.CopyToClipboard(stringBuilder.ToString());
            sender.Hide();
            await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.AppInformation, copyResult));
        }
    }
}
