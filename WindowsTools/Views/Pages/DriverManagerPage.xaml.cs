using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WindowsTools.Extensions.DataType.Class;
using WindowsTools.Extensions.DataType.Enums;
using WindowsTools.Helpers.Root;
using WindowsTools.Models;
using WindowsTools.Services.Root;
using WindowsTools.UI.Dialogs;
using WindowsTools.UI.TeachingTips;
using WindowsTools.Views.Windows;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;
using WindowsTools.WindowsAPI.PInvoke.NewDev;
using WindowsTools.WindowsAPI.PInvoke.Setupapi;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// 驱动管理页面
    /// </summary>
    public sealed partial class DriverManagerPage : Page, INotifyPropertyChanged
    {
        private static Dictionary<string, DEVPROPKEY> DevPropKeyDict { get; } = new()
        {
            { "DEVPKEY_NAME", new DEVPROPKEY() { fmtid = new ("B725F130-47EF-101A-A5F1-02608C9EEBAC"), pid = 10 }},
            { "DEVPKEY_Device_DeviceDesc", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 2 }},
            { "DEVPKEY_Device_HardwareIds", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 3 }},
            { "DEVPKEY_Device_CompatibleIds", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 4 }},
            { "DEVPKEY_Device_Service", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 6 }},
            { "DEVPKEY_Device_Class", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 9 }},
            { "DEVPKEY_Device_ClassGuid", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 10 }},
            { "DEVPKEY_Device_Driver", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 11 }},
            { "DEVPKEY_Device_ConfigFlags", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 12 }},
            { "DEVPKEY_Device_Manufacturer", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 13 }},
            { "DEVPKEY_Device_FriendlyName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 14 }},
            { "DEVPKEY_Device_LocationInfo", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 15 }},
            { "DEVPKEY_Device_PDOName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 16 }},
            { "DEVPKEY_Device_Capabilities", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 17 }},
            { "DEVPKEY_Device_UINumber", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 18 }},
            { "DEVPKEY_Device_UpperFilters", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 19 }},
            { "DEVPKEY_Device_LowerFilters", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 20 }},
            { "DEVPKEY_Device_BusTypeGd", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 21 }},
            { "DEVPKEY_Device_LegacyBusType", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 22 }},
            { "DEVPKEY_Device_BusNumber", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 23 }},
            { "DEVPKEY_Device_EnumeratorName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 24 }},
            { "DEVPKEY_Device_Security", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 25 }},
            { "DEVPKEY_Device_SecuritySDS", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 26 }},
            { "DEVPKEY_Device_DevType", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 27 }},
            { "DEVPKEY_Device_Exclusive", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 28 }},
            { "DEVPKEY_Device_Characteristics", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 29 }},
            { "DEVPKEY_Device_Address", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 30 }},
            { "DEVPKEY_Device_UINumberDescFormat", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 31 }},
            { "DEVPKEY_Device_PowerData", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 32 }},
            { "DEVPKEY_Device_RemovalPolicy", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 33 }},
            { "DEVPKEY_Device_RemovalPolicyDefault", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 34 }},
            { "DEVPKEY_Device_RemovalPolicyOverride", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 35 }},
            { "DEVPKEY_Device_InstallState", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 36 }},
            { "DEVPKEY_Device_LocationPaths", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 37 }},
            { "DEVPKEY_Device_BaseContainerId", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 38 }},
            { "DEVPKEY_Device_DevNodeStatus", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 2 }},
            { "DEVPKEY_Device_ProblemCode", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 3 }},
            { "DEVPKEY_Device_EjectionRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 4 }},
            { "DEVPKEY_Device_RemovalRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 5 }},
            { "DEVPKEY_Device_PowerRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 6 }},
            { "DEVPKEY_Device_BusRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 7 }},
            { "DEVPKEY_Device_Parent", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 8 }},
            { "DEVPKEY_Device_Children", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 9 }},
            { "DEVPKEY_Device_Siblings", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 10 }},
            { "DEVPKEY_Device_TransportRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 11 }},
            { "DEVPKEY_Device_ProblemStatus", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 12 }},
            { "DEVPKEY_Device_Reported", new DEVPROPKEY() { fmtid = new ("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 2 }},
            { "DEVPKEY_Device_Legacy", new DEVPROPKEY() { fmtid = new ("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 3 }},
            { "DEVPKEY_Device_ContainerId", new DEVPROPKEY() { fmtid = new ("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 2 }},
            { "DEVPKEY_Device_InLocalMachineContainer", new DEVPROPKEY() { fmtid = new ("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 4 }},
            { "DEVPKEY_Device_ModelId", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 2 }},
            { "DEVPKEY_Device_FriendlyNameAttributes", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 3 }},
            { "DEVPKEY_Device_ManufacturerAttributes", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 4 }},
            { "DEVPKEY_Device_PresenceNotForDevice", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 5 }},
            { "DEVPKEY_Device_SignalStrength", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 6 }},
            { "DEVPKEY_Device_IsAssociateableByUserAction", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 7 }},
            { "DEVPKEY_Numa_Proximity_Domain", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 1 }},
            { "DEVPKEY_Device_DHP_Rebalance_Policy", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 2 }},
            { "DEVPKEY_Device_Numa_Node", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 3 }},
            { "DEVPKEY_Device_BusReportedDeviceDesc", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 4 }},
            { "DEVPKEY_Device_IsPresent", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 5 }},
            { "DEVPKEY_Device_HasProblem", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 6 }},
            { "DEVPKEY_Device_ConfigurationId", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 7 }},
            { "DEVPKEY_Device_ReportedDeviceIdsHash", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 8 }},
            { "DEVPKEY_Device_PhysicalDeviceLocation", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 9 }},
            { "DEVPKEY_Device_BiosDeviceName", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 10 }},
            { "DEVPKEY_Device_DriverProblemDesc", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 11 }},
            { "DEVPKEY_Device_DebuggerSafe", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 12 }},
            { "DEVPKEY_Device_SessionId", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 6 }},
            { "DEVPKEY_Device_InstallDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 100 }},
            { "DEVPKEY_Device_FirstInstallDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 101 }},
            { "DEVPKEY_Device_LastArrivalDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 102 }},
            { "DEVPKEY_Device_LastRemovalDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 103 }},
            { "DEVPKEY_Device_DriverDate", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 2 }},
            { "DEVPKEY_Device_DriverVersion", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 3 }},
            { "DEVPKEY_Device_DriverDesc", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 4 }},
            { "DEVPKEY_Device_DriverInfPath", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 5 }},
            { "DEVPKEY_Device_DriverInfSection", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 6 }},
            { "DEVPKEY_Device_DriverInfSectionExt", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 7 }},
            { "DEVPKEY_Device_MatchingDeviceId", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 8 }},
            { "DEVPKEY_Device_DriverProvider", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 9 }},
            { "DEVPKEY_Device_DriverPropPageProvider", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 10 }},
            { "DEVPKEY_Device_DriverCoInstallers", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 11 }},
            { "DEVPKEY_Device_ResourcePickerTags", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 12 }},
            { "DEVPKEY_Device_ResourcePickerExceptions", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 13 }},
            { "DEVPKEY_Device_DriverRank", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 14 }},
            { "DEVPKEY_Device_DriverLogoLevel", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 15 }},
            { "DEVPKEY_Device_NoConnectSound", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 17 }},
            { "DEVPKEY_Device_GenericDriverInstalled", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 18 }},
            { "DEVPKEY_Device_AdditionalSoftwareRequested", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 19 }},
            { "DEVPKEY_Device_SafeRemovalRequired", new DEVPROPKEY() { fmtid = new ("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 2 }},
            { "DEVPKEY_Device_SafeRemovalRequiredOverride", new DEVPROPKEY() { fmtid = new ("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 3 }},
            { "DEVPKEY_DrvPkg_Model", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 2 }},
            { "DEVPKEY_DrvPkg_VendorWebSite", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 3 }},
            { "DEVPKEY_DrvPkg_DetailedDescription", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 4 }},
            { "DEVPKEY_DrvPkg_DocumentationLink", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 5 }},
            { "DEVPKEY_DrvPkg_Icon", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 6 }},
            { "DEVPKEY_DrvPkg_BrandingIcon", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 7 }},
            { "DEVPKEY_DeviceClass_UpperFilters", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 19 }},
            { "DEVPKEY_DeviceClass_LowerFilters", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 20 }},
            { "DEVPKEY_DeviceClass_Security", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 25 }},
            { "DEVPKEY_DeviceClass_SecuritySDS", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 26 }},
            { "DEVPKEY_DeviceClass_DevType", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 27 }},
            { "DEVPKEY_DeviceClass_Exclusive", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 28 }},
            { "DEVPKEY_DeviceClass_Characteristics", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 29 }},
            { "DEVPKEY_DeviceClass_Name", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassName", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 3 }},
            { "DEVPKEY_DeviceClass_Icon", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 4 }},
            { "DEVPKEY_DeviceClass_ClassInstaller", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 5 }},
            { "DEVPKEY_DeviceClass_PropPageProvider", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 6 }},
            { "DEVPKEY_DeviceClass_NoInstallClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 7 }},
            { "DEVPKEY_DeviceClass_NoDisplayClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 8 }},
            { "DEVPKEY_DeviceClass_SilentInstall", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 9 }},
            { "DEVPKEY_DeviceClass_NoUseClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 10 }},
            { "DEVPKEY_DeviceClass_DefaultService", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 11 }},
            { "DEVPKEY_DeviceClass_IconPath", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 12 }},
            { "DEVPKEY_DeviceClass_DHPRebalanceOptOut", new DEVPROPKEY() { fmtid = new ("D14D3EF3-66CF-4BA2-9D38-0DDB37AB4701"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassCoInstallers", new DEVPROPKEY() { fmtid = new ("713D1703-A2E2-49F5-9214-56472EF3DA5C"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_FriendlyName", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_Enabled", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 3 }},
            { "DEVPKEY_DeviceInterface_ClassGuid", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 4 }},
            { "DEVPKEY_DeviceInterface_ReferenceString", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 5 }},
            { "DEVPKEY_DeviceInterface_Restricted", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 6 }},
            { "DEVPKEY_DeviceInterfaceClass_DefaultInterface", new DEVPROPKEY() { fmtid = new ("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 2 }},
            { "DEVPKEY_DeviceInterfaceClass_Name", new DEVPROPKEY() { fmtid = new ("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 3 }},
            { "DEVPKEY_Device_Model", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 39 }},
            { "DEVPKEY_DeviceContainer_Address", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 51 }},
            { "DEVPKEY_DeviceContainer_DiscoveryMethod", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 52 }},
            { "DEVPKEY_DeviceContainer_IsEncrypted", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 53 }},
            { "DEVPKEY_DeviceContainer_IsAuthenticated", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 54 }},
            { "DEVPKEY_DeviceContainer_IsConnected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 55 }},
            { "DEVPKEY_DeviceContainer_IsPaired", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 56 }},
            { "DEVPKEY_DeviceContainer_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 57 }},
            { "DEVPKEY_DeviceContainer_Version", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 65 }},
            { "DEVPKEY_DeviceContainer_Last_Seen", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 66 }},
            { "DEVPKEY_DeviceContainer_Last_Connected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 67 }},
            { "DEVPKEY_DeviceContainer_IsShowInDisconnectedState", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 68 }},
            { "DEVPKEY_DeviceContainer_IsLocalMachine", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 70 }},
            { "DEVPKEY_DeviceContainer_MetadataPath", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 71 }},
            { "DEVPKEY_DeviceContainer_IsMetadataSearchInProgress", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 72 }},
            { "DEVPKEY_DeviceContainer_MetadataChecksum", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 73 }},
            { "DEVPKEY_DeviceContainer_IsNotInterestingForDisplay", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 74 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageOnDeviceConnect", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 76 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageFromExplorer", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 77 }},
            { "DEVPKEY_DeviceContainer_BaselineExperienceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 78 }},
            { "DEVPKEY_DeviceContainer_IsDeviceUniquelyIdentifiable", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 79 }},
            { "DEVPKEY_DeviceContainer_AssociationArray", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 80 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription1", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 81 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription2", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 82 }},
            { "DEVPKEY_DeviceContainer_HasProblem", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 83 }},
            { "DEVPKEY_DeviceContainer_IsSharedDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 84 }},
            { "DEVPKEY_DeviceContainer_IsNetworkDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 85 }},
            { "DEVPKEY_DeviceContainer_IsDefaultDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 86 }},
            { "DEVPKEY_DeviceContainer_MetadataCabinet", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 87 }},
            { "DEVPKEY_DeviceContainer_RequiresPairingElevation", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 88 }},
            { "DEVPKEY_DeviceContainer_ExperienceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 89 }},
            { "DEVPKEY_DeviceContainer_Category", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 90 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Singular", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 91 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Plural", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 92 }},
            { "DEVPKEY_DeviceContainer_Category_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 93 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Desc", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 94 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 95 }},
            { "DEVPKEY_DeviceContainer_PrimaryCategory", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 97 }},
            { "DEVPKEY_DeviceContainer_UnpairUninstall", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 98 }},
            { "DEVPKEY_DeviceContainer_RequiresUninstallElevation", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 99 }},
            { "DEVPKEY_DeviceContainer_DeviceFunctionSubRank", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 100 }},
            { "DEVPKEY_DeviceContainer_AlwaysShowDeviceAsConnected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 101 }},
            { "DEVPKEY_DeviceContainer_ConfigFlags", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 105 }},
            { "DEVPKEY_DeviceContainer_PrivilegedPackageFamilyNames", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 106 }},
            { "DEVPKEY_DeviceContainer_CustomPrivilegedPackageFamilyNames", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 107 }},
            { "DEVPKEY_DeviceContainer_IsRebootRequired", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 108 }},
            { "DEVPKEY_Device_InstanceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 256 }},
            { "DEVPKEY_DeviceContainer_FriendlyName", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 12288 }},
            { "DEVPKEY_DeviceContainer_Manufacturer", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8192 }},
            { "DEVPKEY_DeviceContainer_ModelName", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8194 }},
            { "DEVPKEY_DeviceContainer_ModelNumber", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8195 }},
            { "DEVPKEY_DeviceContainer_InstallInProgress", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 9 }}
        };

        private readonly object driverListObject = new();
        private readonly List<DriverModel> driverList = [];
        private readonly string Unknown = ResourceService.DriverManagerResource.GetString("Unknown");
        private readonly string UnknownDeviceName = ResourceService.DriverManagerResource.GetString("UnknownDeviceName");

        private bool isLoaded;

        private bool _isLoadCompleted;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private string _searchDriverNameText = string.Empty;

        public string SearchDriverNameText
        {
            get { return _searchDriverNameText; }

            set
            {
                if (!Equals(_searchDriverNameText, value))
                {
                    _searchDriverNameText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchDriverNameText)));
                }
            }
        }

        private bool _isDriverEmpty = false;

        public bool IsDriverEmpty
        {
            get { return _isDriverEmpty; }

            set
            {
                if (!Equals(_isDriverEmpty, value))
                {
                    _isDriverEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDriverEmpty)));
                }
            }
        }

        private bool _isSearchEmpty = false;

        public bool IsSearchEmpty
        {
            get { return _isSearchEmpty; }

            set
            {
                if (!Equals(_isSearchEmpty, value))
                {
                    _isSearchEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchEmpty)));
                }
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private DriverSortRuleKind _selectedRule = DriverSortRuleKind.DeviceName;

        public DriverSortRuleKind SelectedRule
        {
            get { return _selectedRule; }

            set
            {
                if (!Equals(_selectedRule, value))
                {
                    _selectedRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRule)));
                }
            }
        }

        private bool _isOperating = false;

        public bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperating)));
                }
            }
        }

        private ObservableCollection<DriverModel> DriverCollection { get; } = [];

        private ObservableCollection<DriverOperationModel> DriverOperationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DriverManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 打开文件夹
        /// </summary>
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string driverLocation)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(File.Exists(driverLocation) ? Path.GetDirectoryName(driverLocation) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, "Open driver location failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async void OnDeleteDriverExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driverItem && File.Exists(driverItem.DriverLocation))
            {
                IsOperating = true;

                foreach (DriverModel item in DriverCollection)
                {
                    item.IsOperating = true;
                    if (item.DriverOEMInfName.Equals(driverItem.DriverOEMInfName))
                    {
                        item.IsProcessing = true;
                    }
                }

                Guid driverOperationGuid = Guid.NewGuid();

                DriverOperationCollection.Add(new DriverOperationModel
                {
                    DriverInfName = driverItem.DriverInfName,
                    DriverOperation = ResourceService.DriverManagerResource.GetString("DeletingDriver"),
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                });

                // 删除驱动
                Tuple<bool, Win32Exception> deleteResult = await Task.Run(() =>
                {
                    bool result = SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_NONE, IntPtr.Zero);
                    return result ? Tuple.Create<bool, Win32Exception>(result, null) : Tuple.Create(result, new Win32Exception(Kernel32Library.GetLastError()));
                });

                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                {
                    if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                    {
                        driverOperationItem.IsOperating = false;
                        driverOperationItem.DriverOperation = deleteResult.Item1 ? ResourceService.DriverManagerResource.GetString("DeletingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("DeletingDriverFailed"), deleteResult.Item2.NativeErrorCode, deleteResult.Item2.HResult, deleteResult.Item2.Message);
                        break;
                    }
                }

                IsOperating = false;

                if (deleteResult.Item1)
                {
                    IsLoadCompleted = false;

                    MainWindow.Current.BeginInvoke(async () =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteDriverSuccessfully));
                    });

                    await Task.Run(() =>
                    {
                        List<DriverModel> driverInformationList = GetDriverInformationList();
                        driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                        lock (driverListObject)
                        {
                            driverList.Clear();
                            driverList.AddRange(driverInformationList);
                        }
                    });

                    InitializeData();
                }
                else
                {
                    foreach (DriverModel item in DriverCollection)
                    {
                        item.IsOperating = false;
                        if (item.DriverOEMInfName.Equals(driverItem.DriverOEMInfName))
                        {
                            item.IsProcessing = false;
                        }
                    }

                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async void OnForceDeleteDriverExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driverItem && File.Exists(driverItem.DriverLocation))
            {
                IsOperating = true;

                foreach (DriverModel item in DriverCollection)
                {
                    item.IsOperating = true;
                    if (item.DriverOEMInfName.Equals(driverItem.DriverOEMInfName))
                    {
                        item.IsProcessing = true;
                    }
                }

                Guid driverOperationGuid = Guid.NewGuid();

                DriverOperationCollection.Add(new DriverOperationModel
                {
                    DriverInfName = driverItem.DriverInfName,
                    DriverOperation = ResourceService.DriverManagerResource.GetString("ForceDeletingDriver"),
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                });

                // 强制删除驱动
                bool needReboot = false;
                Tuple<bool, Win32Exception> deleteResult = await Task.Run(() =>
                {
                    bool result = NewDevLibrary.DiUninstallDriver(IntPtr.Zero, driverItem.DriverLocation, DIURFLAG.NO_REMOVE_INF, out needReboot) && SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, IntPtr.Zero);
                    return result ? Tuple.Create<bool, Win32Exception>(result, null) : Tuple.Create(result, new Win32Exception(Kernel32Library.GetLastError()));
                });

                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                {
                    if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                    {
                        driverOperationItem.IsOperating = false;
                        driverOperationItem.DriverOperation = deleteResult.Item1 ? ResourceService.DriverManagerResource.GetString("ForceDeletingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("ForceDeletingDriverFailed"), deleteResult.Item2.NativeErrorCode, deleteResult.Item2.HResult, deleteResult.Item2.Message);
                        break;
                    }
                }

                IsOperating = false;

                if (deleteResult.Item1)
                {
                    IsLoadCompleted = false;

                    MainWindow.Current.BeginInvoke(async () =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ForceDeleteDriverSuccessfully));
                    });

                    if (needReboot)
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.UnInstallDriver));

                            await Task.Run(() =>
                            {
                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    ShutdownHelper.Restart(ResourceService.DriverManagerResource.GetString("RestartPC"), TimeSpan.FromSeconds(120));
                                }
                            });
                        });
                    }

                    await Task.Run(() =>
                    {
                        List<DriverModel> driverInformationList = GetDriverInformationList();
                        driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                        lock (driverListObject)
                        {
                            driverList.Clear();
                            driverList.AddRange(driverInformationList);
                        }
                    });

                    InitializeData();
                }
                else
                {
                    foreach (DriverModel item in DriverCollection)
                    {
                        item.IsOperating = false;
                        if (item.DriverOEMInfName.Equals(driverItem.DriverOEMInfName))
                        {
                            item.IsProcessing = false;
                        }
                    }

                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ForceDeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 删除驱动任务
        /// </summary>
        private void OnDeleteDriverOperationTaskExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Guid driverOperationGuid)
            {
                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                {
                    if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid) && !driverOperationItem.IsOperating)
                    {
                        DriverOperationCollection.Remove(driverOperationItem);
                        break;
                    }
                }
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 加载完成后初始化内容
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                IsLoadCompleted = false;

                await Task.Run(() =>
                {
                    List<DriverModel> driverInformationList = GetDriverInformationList();
                    driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                    lock (driverListObject)
                    {
                        driverList.Clear();
                        driverList.AddRange(driverInformationList);
                    }
                });

                InitializeData();
            }
        }

        /// <summary>
        /// 打开设备管理器
        /// </summary>

        private void OnOpenDeviceManagementClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("devmgmt.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, "Open device management failed", e);
                }
            });
        }

        /// <summary>
        /// 清空驱动操作任务（只允许清理未正进行的驱动操作任务）
        /// </summary>

        private void OnClearDriverOperationTaskClicked(object sender, RoutedEventArgs args)
        {
            for (int index = DriverOperationCollection.Count - 1; index >= 0; index--)
            {
                if (!DriverOperationCollection[index].IsOperating)
                {
                    DriverOperationCollection.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (TaskManagerFlyout.IsOpen)
            {
                TaskManagerFlyout.Hide();
            }
        }

        /// <summary>
        /// 搜索驱动名称
        /// </summary>
        private void OnSearchDriverNameQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchDriverNameText))
            {
                IsLoadCompleted = false;
                InitializeData();
            }
        }

        /// <summary>
        /// 搜索驱动名称内容发生变化事件
        /// </summary>
        private void OnSerachDriverNameTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchDriverNameText = sender.Text;

            if (string.IsNullOrEmpty(SearchDriverNameText))
            {
                IsLoadCompleted = false;
                InitializeData();
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DriverModel driverItem in DriverCollection)
            {
                driverItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (DriverModel driverItem in DriverCollection)
            {
                driverItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                IsIncrease = Convert.ToBoolean(radioMenuFlyoutItem.Tag);
                IsLoadCompleted = false;
                InitializeData();
            }
        }

        /// <summary>
        /// 排序规则
        /// </summary>
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedRule = (DriverSortRuleKind)radioMenuFlyoutItem.Tag;
                IsLoadCompleted = false;
                InitializeData();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsLoadCompleted = false;

            await Task.Run(() =>
            {
                List<DriverModel> driverInformationList = GetDriverInformationList();
                driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                lock (driverListObject)
                {
                    driverList.Clear();
                    driverList.AddRange(driverInformationList);
                }
            });

            InitializeData();
        }

        /// <summary>
        /// 添加驱动
        /// </summary>
        private async void OnAddDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                OpenFileDialog dialog = new()
                {
                    Multiselect = true,
                    Title = ResourceService.DriverManagerResource.GetString("SelectFile"),
                    Filter = ResourceService.DriverManagerResource.GetString("DriverFilterCondition"),
                };

                if (dialog.ShowDialog() is DialogResult.OK)
                {
                    IsOperating = true;

                    foreach (DriverModel driverItem in DriverCollection)
                    {
                        driverItem.IsOperating = true;
                    }

                    List<Task> addDriverTaskList = [];
                    Dictionary<Guid, Tuple<string, bool, string>> addDriverResultDict = [];
                    object addDriverResultDictObject = new();

                    foreach (string fileName in dialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();

                        DriverOperationCollection.Add(new DriverOperationModel
                        {
                            DriverInfName = fileName,
                            DriverOperation = ResourceService.DriverManagerResource.GetString("AddingDriver"),
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        });

                        addDriverTaskList.Add(Task.Run(() =>
                        {
                            StringBuilder stringBuilder = new(260);
                            uint bufferSize = 0;
                            bool result = SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, IntPtr.Zero);
                            Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                            string driverOperation = result ? ResourceService.DriverManagerResource.GetString("AddingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("AddingDriverFailed"), win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                            lock (addDriverResultDictObject)
                            {
                                addDriverResultDict.Add(driverOperationGuid, Tuple.Create(fileName, result, driverOperation));
                            }

                            MainWindow.Current.BeginInvoke(() =>
                            {
                                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                                {
                                    if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                                    {
                                        driverOperationItem.DriverOperation = driverOperation;
                                        driverOperationItem.IsOperating = false;
                                        break;
                                    }
                                }
                            });
                        }));
                    }

                    await Task.WhenAll([.. addDriverTaskList]);
                    IsOperating = false;

                    if (addDriverResultDict.Values.Any(item => item.Item2))
                    {
                        IsLoadCompleted = false;

                        if (addDriverResultDict.Values.All(item => item.Item2))
                        {
                            MainWindow.Current.BeginInvoke(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddDriverAllSuccessfully));
                            });
                        }
                        else
                        {
                            MainWindow.Current.BeginInvoke(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddDriverPartialSuccessfully));
                            });
                        }

                        await Task.Run(() =>
                        {
                            List<DriverModel> driverInformationList = GetDriverInformationList();
                            driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                            lock (driverListObject)
                            {
                                driverList.Clear();
                                driverList.AddRange(driverInformationList);
                            }
                        });

                        InitializeData();
                    }
                    else
                    {
                        foreach (DriverModel driverItem in DriverCollection)
                        {
                            driverItem.IsOperating = false;
                        }

                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddDriverFailed));
                    }
                }
            }
        }

        /// <summary>
        /// 添加并安装驱动
        /// </summary>
        private async void OnAddInstallDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                OpenFileDialog dialog = new()
                {
                    Multiselect = true,
                    Title = ResourceService.DriverManagerResource.GetString("SelectFile"),
                    Filter = ResourceService.DriverManagerResource.GetString("DriverFilterCondition"),
                };

                if (dialog.ShowDialog() is DialogResult.OK)
                {
                    bool needReboot = false;
                    IsOperating = true;

                    foreach (DriverModel driverItem in DriverCollection)
                    {
                        driverItem.IsOperating = true;
                    }

                    List<Task> addInstallDriverTaskList = [];
                    Dictionary<Guid, Tuple<string, bool, string>> addInstallDriverResultDict = [];
                    object addInstallDriverResultDictObject = new();

                    foreach (string fileName in dialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();

                        DriverOperationCollection.Add(new DriverOperationModel
                        {
                            DriverInfName = fileName,
                            DriverOperation = ResourceService.DriverManagerResource.GetString("AddInstallingDriver"),
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        });

                        addInstallDriverTaskList.Add(Task.Run(() =>
                        {
                            StringBuilder stringBuilder = new(260);
                            uint bufferSize = 0;
                            bool result = NewDevLibrary.DiInstallDriver(IntPtr.Zero, fileName, 0, out bool NeedReboot) && SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, IntPtr.Zero);
                            Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                            string driverOperation = result ? ResourceService.DriverManagerResource.GetString("AddInstallingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("AddInstallingDriverFailed"), win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                            lock (addInstallDriverResultDictObject)
                            {
                                addInstallDriverResultDict.Add(driverOperationGuid, Tuple.Create(fileName, result, driverOperation));
                            }

                            MainWindow.Current.BeginInvoke(() =>
                            {
                                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                                {
                                    if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                                    {
                                        driverOperationItem.DriverOperation = driverOperation;
                                        driverOperationItem.IsOperating = false;
                                        break;
                                    }
                                }
                            });
                        }));
                    }

                    await Task.WhenAll([.. addInstallDriverTaskList]);
                    IsOperating = false;

                    if (addInstallDriverResultDict.Values.Any(item => item.Item2))
                    {
                        IsLoadCompleted = false;

                        if (addInstallDriverResultDict.Values.All(item => item.Item2))
                        {
                            MainWindow.Current.BeginInvoke(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddInstallDriverAllSuccessfully));
                            });
                        }
                        else
                        {
                            MainWindow.Current.BeginInvoke(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddInstallDriverPartialSuccessfully));
                            });
                        }

                        if (needReboot)
                        {
                            MainWindow.Current.BeginInvoke(async () =>
                            {
                                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.InstallDriver));

                                await Task.Run(() =>
                                {
                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        ShutdownHelper.Restart(ResourceService.DriverManagerResource.GetString("RestartPC"), TimeSpan.FromSeconds(120));
                                    }
                                });
                            });
                        }

                        await Task.Run(() =>
                        {
                            List<DriverModel> driverInformationList = GetDriverInformationList();
                            driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                            lock (driverListObject)
                            {
                                driverList.Clear();
                                driverList.AddRange(driverInformationList);
                            }
                        });

                        InitializeData();
                    }
                    else
                    {
                        foreach (DriverModel driverItem in DriverCollection)
                        {
                            driverItem.IsOperating = false;
                        }

                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.AddInstallDriverFailed));
                    }
                }
            }
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async void OnDeleteDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                List<DriverModel> selectedDriverList = [.. DriverCollection.Where(item => item.IsSelected)];
                IsOperating = true;

                foreach (DriverModel driverItem in DriverCollection)
                {
                    driverItem.IsOperating = true;
                    foreach (DriverModel selectedDriverItem in selectedDriverList)
                    {
                        if (driverItem.DriverOEMInfName.Equals(selectedDriverItem.DriverOEMInfName))
                        {
                            driverItem.IsProcessing = true;
                            break;
                        }
                    }
                }

                List<Task> deleteDriverTaskList = [];
                Dictionary<Guid, Tuple<string, bool, string>> deleteDriverResultDict = [];
                object deleteDriverResultDictLock = new();

                foreach (DriverModel driverItem in selectedDriverList)
                {
                    Guid driverOperationGuid = Guid.NewGuid();

                    DriverOperationCollection.Add(new DriverOperationModel
                    {
                        DriverInfName = driverItem.DriverInfName,
                        DriverOperation = ResourceService.DriverManagerResource.GetString("DeletingDriver"),
                        DriverOperationGuid = driverOperationGuid,
                        IsOperating = true
                    });

                    deleteDriverTaskList.Add(Task.Run(() =>
                    {
                        bool result = SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_NONE, IntPtr.Zero);
                        Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                        string driverOperation = result ? ResourceService.DriverManagerResource.GetString("DeletingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("DeletingDriverFailed"), win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        lock (deleteDriverResultDictLock)
                        {
                            deleteDriverResultDict.Add(driverOperationGuid, Tuple.Create(driverItem.DriverInfName, result, driverOperation));
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                            {
                                if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                                {
                                    driverOperationItem.DriverOperation = driverOperation;
                                    driverOperationItem.IsOperating = false;
                                    break;
                                }
                            }
                        });
                    }));
                }

                await Task.WhenAll([.. deleteDriverTaskList]);
                IsOperating = false;

                if (deleteDriverResultDict.Values.Any(item => item.Item2))
                {
                    IsLoadCompleted = false;

                    if (deleteDriverResultDict.Values.All(item => item.Item2))
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteDriverAllSuccessfully));
                        });
                    }
                    else
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteDriverPartialSuccessfully));
                        });
                    }

                    await Task.Run(() =>
                    {
                        List<DriverModel> driverInformationList = GetDriverInformationList();
                        driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                        lock (driverListObject)
                        {
                            driverList.Clear();
                            driverList.AddRange(driverInformationList);
                        }
                    });

                    InitializeData();
                }
                else
                {
                    foreach (DriverModel driverItem in DriverCollection)
                    {
                        driverItem.IsOperating = false;
                        foreach (DriverModel selectedDriverItem in selectedDriverList)
                        {
                            if (driverItem.DriverOEMInfName.Equals(selectedDriverItem.DriverOEMInfName))
                            {
                                driverItem.IsProcessing = false;
                                break;
                            }
                        }
                    }

                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.DeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async void OnForceDeleteDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                List<DriverModel> selectedDriverList = [.. DriverCollection.Where(item => item.IsSelected)];

                bool needReboot = false;
                IsOperating = true;

                foreach (DriverModel driverItem in DriverCollection)
                {
                    driverItem.IsOperating = true;
                    foreach (DriverModel selectedDriverItem in selectedDriverList)
                    {
                        if (driverItem.DriverOEMInfName.Equals(selectedDriverItem.DriverOEMInfName))
                        {
                            driverItem.IsProcessing = true;
                            break;
                        }
                    }
                }

                List<Task> forceDeleteDriverTaskList = [];
                Dictionary<Guid, Tuple<string, bool, string>> forceDeleteDriverResultDict = [];
                object forceDeleteDriverResultDictLock = new();

                foreach (DriverModel driverItem in selectedDriverList)
                {
                    Guid driverOperationGuid = Guid.NewGuid();

                    DriverOperationCollection.Add(new DriverOperationModel
                    {
                        DriverInfName = driverItem.DriverInfName,
                        DriverOperation = ResourceService.DriverManagerResource.GetString("ForceDeletingDriver"),
                        DriverOperationGuid = driverOperationGuid,
                        IsOperating = true
                    });

                    forceDeleteDriverTaskList.Add(Task.Run(() =>
                    {
                        bool result = NewDevLibrary.DiUninstallDriver(IntPtr.Zero, driverItem.DriverLocation, DIURFLAG.NO_REMOVE_INF, out bool NeedReboot) && SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, IntPtr.Zero);
                        Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                        string driverOperation = result ? ResourceService.DriverManagerResource.GetString("ForceDeletingDriverSuccessfully") : string.Format(ResourceService.DriverManagerResource.GetString("ForceDeletingDriverFailed"), win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        lock (forceDeleteDriverResultDictLock)
                        {
                            forceDeleteDriverResultDict.Add(driverOperationGuid, Tuple.Create(driverItem.DriverInfName, result, driverOperation));
                        }

                        if (NeedReboot)
                        {
                            needReboot = NeedReboot;
                        }

                        MainWindow.Current.BeginInvoke(() =>
                        {
                            foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                            {
                                if (driverOperationItem.DriverOperationGuid.Equals(driverOperationGuid))
                                {
                                    driverOperationItem.DriverOperation = driverOperation;
                                    driverOperationItem.IsOperating = false;
                                    break;
                                }
                            }
                        });
                    }));
                }

                await Task.WhenAll([.. forceDeleteDriverTaskList]);
                IsOperating = false;

                if (forceDeleteDriverResultDict.Values.Any(item => item.Item2))
                {
                    IsLoadCompleted = false;

                    if (forceDeleteDriverResultDict.Values.All(item => item.Item2))
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ForceDeleteDriverAllSuccessfully));
                        });
                    }
                    else
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ForceDeleteDriverPartialSuccessfully));
                        });
                    }

                    // 添加重启提示
                    if (needReboot)
                    {
                        MainWindow.Current.BeginInvoke(async () =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.UnInstallDriver));

                            await Task.Run(() =>
                            {
                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    ShutdownHelper.Restart(ResourceService.DriverManagerResource.GetString("RestartPC"), TimeSpan.FromSeconds(120));
                                }
                            });
                        });
                    }

                    await Task.Run(() =>
                    {
                        List<DriverModel> driverInformationList = GetDriverInformationList();
                        driverInformationList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));

                        lock (driverListObject)
                        {
                            driverList.Clear();
                            driverList.AddRange(driverInformationList);
                        }
                    });

                    InitializeData();
                }
                else
                {
                    foreach (DriverModel driverItem in DriverCollection)
                    {
                        driverItem.IsOperating = false;
                        foreach (DriverModel selectedDriverItem in selectedDriverList)
                        {
                            if (driverItem.DriverOEMInfName.Equals(selectedDriverItem.DriverOEMInfName))
                            {
                                driverItem.IsProcessing = false;
                                break;
                            }
                        }
                    }

                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ForceDeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 选择旧驱动
        /// </summary>
        private void OnSelectOldDriverClicked(object sender, RoutedEventArgs args)
        {
            IEnumerable<IGrouping<string, DriverModel>> driverInfGroupInfoList = DriverCollection.GroupBy(item => item.DriverInfName);

            foreach (IGrouping<string, DriverModel> driverInfGroupInfo in driverInfGroupInfoList)
            {
                if (driverInfGroupInfo.Count() <= 1)
                {
                    continue;
                }

                DateTime lastestDate = driverInfGroupInfo.Max(item => item.DriverDate);

                foreach (DriverModel driverItem in driverInfGroupInfo)
                {
                    if (driverItem.DriverDate < lastestDate)
                    {
                        driverItem.IsSelected = true;
                    }
                }
            }
        }

        #endregion 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            DriverCollection.Clear();

            if (SelectedRule.Equals(DriverSortRuleKind.DeviceName))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DeviceName) : driverList.OrderByDescending(item => item.DeviceName))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.InfName))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverInfName) : driverList.OrderByDescending(item => item.DriverInfName))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.OEMInfName))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverOEMInfName) : driverList.OrderByDescending(item => item.DriverOEMInfName))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.DeviceType))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverType) : driverList.OrderByDescending(item => item.DriverType))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.Manufacturer))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverManufacturer) : driverList.OrderByDescending(item => item.DriverManufacturer))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.Manufacturer))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverVersion) : driverList.OrderByDescending(item => item.DriverVersion))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }
            else if (SelectedRule.Equals(DriverSortRuleKind.Date))
            {
                foreach (DriverModel driverItem in IsIncrease ? driverList.OrderBy(item => item.DriverDate) : driverList.OrderByDescending(item => item.DriverDate))
                {
                    driverItem.IsSelected = false;

                    if (driverItem.DeviceName.Contains(SearchDriverNameText) || driverItem.DriverInfName.Contains(SearchDriverNameText) || driverItem.DriverOEMInfName.Contains(SearchDriverNameText) || driverItem.DriverManufacturer.Contains(SearchDriverNameText))
                    {
                        DriverCollection.Add(driverItem);
                    }
                }
            }

            IsDriverEmpty = driverList.Count is 0;
            IsSearchEmpty = DriverCollection.Count is 0;
            IsLoadCompleted = true;
        }

        /// <summary>
        /// 获取设备上所有的驱动信息
        /// </summary>
        private List<DriverModel> GetDriverInformationList()
        {
            List<DriverModel> driverList = [];

            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "pnputil.exe",
                    Arguments = @"/enum-drivers /format ""xml""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                Process process = Process.Start(startInfo);
                StreamReader streamReader = process.StandardOutput;
                string driverString = streamReader.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

                XDocument xDocument = XDocument.Parse(driverString);
                List<PnpDriverInformation> pnpDriverInformationList = [];

                if (xDocument is not null)
                {
                    List<XElement> driverNodeList = [.. xDocument.Descendants("Driver")];

                    foreach (XElement driverNode in driverNodeList)
                    {
                        PnpDriverInformation pnpDriverInformation = new();

                        if (driverNode.Attribute("DriverName") is XAttribute driverNameAttribute)
                        {
                            pnpDriverInformation.DriverName = driverNameAttribute.Value;
                        }

                        if (driverNode.Element("OriginalName") is XElement driverNameElement)
                        {
                            pnpDriverInformation.OriginalName = driverNameElement.Value;
                        }

                        if (driverNode.Element("ProviderName") is XElement providerNameElement)
                        {
                            pnpDriverInformation.ProviderName = providerNameElement.Value;
                        }

                        if (driverNode.Element("ClassName") is XElement classNameElement)
                        {
                            pnpDriverInformation.ClassName = classNameElement.Value;
                        }

                        if (driverNode.Element("ClassGuid") is XElement classGuidElement)
                        {
                            pnpDriverInformation.ClassGuid = classGuidElement.Value;
                        }

                        if (driverNode.Element("DriverVersion") is XElement driverVersionElement)
                        {
                            string[] driverVersionArray = driverVersionElement.Value.Split(' ');

                            if (driverVersionArray.Length is 2)
                            {
                                pnpDriverInformation.DriverDate = driverVersionArray[0];
                                pnpDriverInformation.DriverVersion = driverVersionArray[1];
                            }
                        }

                        if (driverNode.Element("SignerName") is XElement signerNameElement)
                        {
                            pnpDriverInformation.SignerName = signerNameElement.Value;
                        }

                        pnpDriverInformationList.Add(pnpDriverInformation);
                    }
                }

                List<SystemDriverInformation> systemDriverInformationList = [];
                IntPtr deviceInfoSet = SetupapiLibrary.SetupDiGetClassDevs(Guid.Empty, null, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES);

                if (deviceInfoSet != IntPtr.Zero)
                {
                    SP_DEVINFO_DATA deviceInfoData = new()
                    {
                        cbSize = Marshal.SizeOf<SP_DEVINFO_DATA>(),
                    };

                    // 枚举所有设备
                    for (int index = 0; SetupapiLibrary.SetupDiEnumDeviceInfo(deviceInfoSet, index, ref deviceInfoData); index++)
                    {
                        object deviceGuid = GetDevNodeProperty("DEVPKEY_Device_ClassGuid", deviceInfoData, deviceInfoSet);
                        object deviceDesc = GetDevNodeProperty("DEVPKEY_Device_DeviceDesc", deviceInfoData, deviceInfoSet);
                        object driverInfPath = GetDevNodeProperty("DEVPKEY_Device_DriverInfPath", deviceInfoData, deviceInfoSet);
                        object driverDate = GetDevNodeProperty("DEVPKEY_Device_DriverDate", deviceInfoData, deviceInfoSet);
                        object driverVersion = GetDevNodeProperty("DEVPKEY_Device_DriverVersion", deviceInfoData, deviceInfoSet);

                        if (deviceGuid is not null && deviceDesc is not null && driverInfPath is not null && driverDate is not null && driverVersion is not null)
                        {
                            systemDriverInformationList.Add(new SystemDriverInformation()
                            {
                                DeviceGuid = deviceGuid is null ? Guid.NewGuid() : Guid.TryParse(Convert.ToString(deviceGuid), out Guid guid) ? guid : Guid.NewGuid(),
                                Description = Convert.ToString(deviceDesc),
                                InfPath = Convert.ToString(driverInfPath),
                                Date = (DateTime)driverDate,
                                Version = new Version(Convert.ToString(driverVersion))
                            });
                        }
                    }

                    SetupapiLibrary.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }

                foreach (PnpDriverInformation pnpDriverInformationItem in pnpDriverInformationList)
                {
                    string driverLocation = GetDriverStoreLocation(pnpDriverInformationItem.DriverName);

                    DriverModel driverItem = new()
                    {
                        DriverInfName = pnpDriverInformationItem.OriginalName,
                        DriverOEMInfName = pnpDriverInformationItem.DriverName,
                        DriverManufacturer = pnpDriverInformationItem.ProviderName,
                        DriverDate = DateTime.Parse(pnpDriverInformationItem.DriverDate),
                        DriverVersion = new Version(pnpDriverInformationItem.DriverVersion),
                        DriverSize = string.IsNullOrEmpty(driverLocation) ? "0B" : FileSizeHelper.ConvertFileSizeToString(GetFolderSize(Path.GetDirectoryName(driverLocation))),
                        DriverLocation = driverLocation,
                        DriverType = pnpDriverInformationItem.ClassName,
                        SignatureName = string.IsNullOrEmpty(pnpDriverInformationItem.SignerName) ? Unknown : pnpDriverInformationItem.SignerName,
                    };

                    Guid dismDriverPackageItemGuid = new(pnpDriverInformationItem.ClassGuid);

                    foreach (SystemDriverInformation systemDriverInformation in systemDriverInformationList)
                    {
                        if (dismDriverPackageItemGuid.Equals(systemDriverInformation.DeviceGuid) && driverItem.DriverOEMInfName.Equals(systemDriverInformation.InfPath, StringComparison.OrdinalIgnoreCase) && driverItem.DriverDate.Equals(systemDriverInformation.Date) && driverItem.DriverVersion.Equals(systemDriverInformation.Version))
                        {
                            driverItem.DeviceName = string.IsNullOrEmpty(systemDriverInformation.Description) ? UnknownDeviceName : systemDriverInformation.Description;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(driverItem.DeviceName))
                    {
                        driverItem.DeviceName = UnknownDeviceName;
                    }

                    driverList.Add(driverItem);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, "Get Device information failed", e);
            }

            return driverList;
        }

        /// <summary>
        /// 检索设备实例属性
        /// </summary>
        private object GetDevNodeProperty(string devPropKey, SP_DEVINFO_DATA deviceInfoData, IntPtr deviceInfoSet)
        {
            object value = null;

            if (DevPropKeyDict.TryGetValue(devPropKey, out DEVPROPKEY devPropKeyItem))
            {
                SetupapiLibrary.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref devPropKeyItem, out _, IntPtr.Zero, 0, out int bufferSize, 0);
                IntPtr propertyBufferPtr = Marshal.AllocHGlobal(bufferSize);

                if (SetupapiLibrary.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref devPropKeyItem, out DEVPROP_TYPE devPropertyType, propertyBufferPtr, bufferSize, out bufferSize, 0))
                {
                    // 空值
                    switch (devPropertyType)
                    {
                        case DEVPROP_TYPE.DEVPROP_TYPE_EMPTY: break;
                        case DEVPROP_TYPE.DEVPROP_TYPE_NULL: break;
                        case DEVPROP_TYPE.DEVPROP_TYPE_SBYTE:
                            {
                                value = Convert.ToSByte(Marshal.ReadByte(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BYTE:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT16:
                            {
                                value = Marshal.ReadInt16(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT16:
                            {
                                value = Convert.ToUInt16(Marshal.ReadInt16(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT32:
                            {
                                value = Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT32:
                            {
                                value = Convert.ToUInt32(Marshal.ReadInt32(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT64:
                            {
                                value = Marshal.ReadInt64(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT64:
                            {
                                value = Convert.ToUInt64(Marshal.ReadInt64(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_FLOAT:
                            {
                                byte[] byteBuffer = new byte[sizeof(float)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = BitConverter.ToSingle(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DOUBLE:
                            {
                                byte[] byteBuffer = new byte[sizeof(double)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = BitConverter.ToDouble(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DECIMAL:
                            {
                                byte[] byteBuffer = new byte[sizeof(decimal)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = Convert.ToDecimal(BitConverter.ToDouble(byteBuffer, 0));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_GUID:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<Guid>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = new Guid(byteBuffer);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_CURRENCY:
                            {
                                value = Marshal.ReadInt64(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DATE:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<System.Runtime.InteropServices.ComTypes.FILETIME>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = DateTime.FromOADate(BitConverter.ToDouble(byteBuffer, 0)).Date;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_FILETIME:
                            {
                                long fileTime = Marshal.ReadInt64(propertyBufferPtr);
                                value = DateTime.FromFileTime(fileTime).Date;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BOOLEAN:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr) is not 0;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_STRING:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<System.Runtime.InteropServices.ComTypes.FILETIME>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = new RawSecurityDescriptor(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPKEY:
                            {
                                DEVPROPKEY key = new();

                                byte[] byteBuffer = new byte[Marshal.SizeOf<Guid>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                key.fmtid = new Guid(byteBuffer);
                                key.pid = Convert.ToUInt32(Marshal.ReadInt32(propertyBufferPtr, 16));
                                value = key;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPTYPE:
                            {
                                value = (DEVPROP_TYPE)Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_ERROR:
                            {
                                value = Marshal.GetExceptionForHR(Marshal.ReadInt32(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_NTSTATUS:
                            {
                                value = Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_STRING_INDIRECT:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPEMOD_ARRAY:
                            {
                                byte[] byteBuffer = new byte[bufferSize];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = byteBuffer;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BINARY:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr);
                                break;
                            }
                        default:
                            {
                                byte[] byteBuffer = new byte[bufferSize];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = byteBuffer;
                                break;
                            }
                    }
                }

                Marshal.FreeHGlobal(propertyBufferPtr);
            }

            return value;
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        public long GetFolderSize(string directoryPath)
        {
            long totalSize = 0;

            if (Directory.Exists(directoryPath))
            {
                try
                {
                    DirectoryInfo directoryInfo = new(directoryPath);
                    long fileAllSize = 0;

                    // 获取当前目录所有文件大小
                    try
                    {
                        FileInfo[] filesArray = directoryInfo.GetFiles();

                        Parallel.ForEach(filesArray, file =>
                        {
                            try
                            {
                                long fileSize = file.Length;
                                Interlocked.Add(ref fileAllSize, fileSize);
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Get directory all file size {0} failed", directoryPath), e);
                    }

                    totalSize += fileAllSize;

                    // 获取当前文件夹包含的所有子目录大小
                    long folderAllSize = 0;

                    try
                    {
                        DirectoryInfo[] directoryArray = directoryInfo.GetDirectories();

                        Parallel.ForEach(directoryArray, directory =>
                        {
                            try
                            {
                                long folderSize = GetFolderSize(directory.FullName);
                                Interlocked.Add(ref folderAllSize, folderSize);
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, string.Format("Get directory all sub directory size {0} failed", directoryPath), e);
                    }

                    totalSize += folderAllSize;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, string.Format("Get directory information {0} failed", directoryPath), e);
                }
            }

            return totalSize;
        }

        /// <summary>
        /// 获取驱动存放的实际路径
        /// </summary>
        public static string GetDriverStoreLocation(string oemInfName)
        {
            SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, IntPtr.Zero, IntPtr.Zero, null, 0, out int bufferSize);

            if (bufferSize > 0)
            {
                StringBuilder stringBuilder = new(bufferSize);
                return SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity, out _) ? stringBuilder.ToString() : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
