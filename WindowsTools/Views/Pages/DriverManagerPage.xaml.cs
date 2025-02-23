using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Helpers.Root;
using WindowsTools.WindowsAPI.PInvoke.CfgMgr32;
using WindowsTools.WindowsAPI.PInvoke.Dismapi;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 驱动管理页面
    /// </summary>
    public sealed partial class DriverManagerPage : Page
    {
        private bool isLoaded;

        public DriverManagerPage()
        {
            InitializeComponent();
        }

        #region 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 加载完成后初始化内容
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isLoaded && RuntimeHelper.IsElevated)
            {
                isLoaded = true;
            }
        }

        #endregion 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 获取设备上所有的驱动信息
        /// </summary>
        public static void GetDriverInformation()
        {
            DismapiLibrary.DismInitialize(DismLogLevel.DismLogErrorsWarningsInfo, null, null);

            try
            {
                DismapiLibrary.DismOpenSession(DismapiLibrary.DISM_ONLINE_IMAGE, null, null, out IntPtr session);
                DismapiLibrary.DismGetDrivers(session, false, out IntPtr driverPackage, out uint count);

                DismDriverPackage[] dismDriverPackageArray = new DismDriverPackage[count];
                for (int index = 0; index < count; index++)
                {
                    IntPtr driverPackageOffsetPtr = IntPtr.Add(driverPackage, index * Marshal.SizeOf<DismDriverPackage>());
                    dismDriverPackageArray[index] = Marshal.PtrToStructure<DismDriverPackage>(driverPackageOffsetPtr);
                }

                if (CfgMgr32Library.CM_Get_Device_ID_List_Size(out int deviceListSize, null, CM_GETIDLIST_FILTER.CM_GETIDLIST_FILTER_NONE) is CR.CR_SUCCESS)
                {
                    byte[] deviceBuffer = new byte[deviceListSize * sizeof(char) + 2];

                    if (CfgMgr32Library.CM_Get_Device_ID_List(null, deviceBuffer, deviceListSize, CM_GETIDLIST_FILTER.CM_GETIDLIST_FILTER_NONE) is CR.CR_SUCCESS)
                    {
                        string[] deviceIdArray = Encoding.Unicode.GetString(deviceBuffer).Split(['\0'], StringSplitOptions.RemoveEmptyEntries);

                        foreach (string deviceId in deviceIdArray)
                        {
                            if (CfgMgr32Library.CM_Locate_DevNode(out uint devInst, deviceId, CM_LOCATE_DEVNODE_FLAGS.CM_LOCATE_DEVNODE_PHANTOM) is CR.CR_SUCCESS)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                DismapiLibrary.DismShutdown();
            }
        }
    }
}
