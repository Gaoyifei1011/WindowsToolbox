using Windows.UI.Xaml.Controls;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Services.Root;

namespace WindowsTools.UI.Dialogs
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    public sealed partial class RebootDialog : ContentDialog
    {
        public RebootDialog(DriverInstallKind driverInstallKind)
        {
            InitializeComponent();
            switch (driverInstallKind)
            {
                case DriverInstallKind.InstallDriver:
                    {
                        Content = ResourceService.DialogResource.GetString("InstallDriverReboot");
                        break;
                    }
                case DriverInstallKind.UnInstallDriver:
                    {
                        Content = ResourceService.DialogResource.GetString("UnInstallDriverReboot");
                        break;
                    }
            }
        }
    }
}
