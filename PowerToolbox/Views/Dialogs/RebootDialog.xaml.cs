using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using Windows.UI.Xaml.Controls;

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 重启设备对话框
    /// </summary>
    public sealed partial class RebootDialog : ContentDialog
    {
        private readonly string InstallDriverRebootString = ResourceService.DialogResource.GetString("InstallDriverReboot");
        private readonly string UnInstallDriverRebootString = ResourceService.DialogResource.GetString("UnInstallDriverReboot");

        public RebootDialog(DriverInstallKind driverInstallKind)
        {
            InitializeComponent();
            switch (driverInstallKind)
            {
                case DriverInstallKind.InstallDriver:
                    {
                        Content = InstallDriverRebootString;
                        break;
                    }
                case DriverInstallKind.UnInstallDriver:
                    {
                        Content = UnInstallDriverRebootString;
                        break;
                    }
            }
        }
    }
}
