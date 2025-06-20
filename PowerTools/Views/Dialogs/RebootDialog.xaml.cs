using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;
using Windows.UI.Xaml.Controls;

namespace PowerTools.Views.Dialogs
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
