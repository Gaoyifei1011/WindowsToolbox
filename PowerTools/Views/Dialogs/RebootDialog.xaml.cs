using Windows.UI.Xaml.Controls;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;

namespace PowerTools.Views.Dialogs
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
