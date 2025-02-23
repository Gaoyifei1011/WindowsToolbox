using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.CfgMgr32
{
    /// <summary>
    /// CfgMgr32.dll 函数库
    /// </summary>
    public static class CfgMgr32Library
    {
        private const string CfgMgr32 = "cfgMgr32.dll";

        /// <summary>
        /// CM_Get_Device_ID_List 函数检索本地计算机设备实例的设备实例 ID 列表。
        /// </summary>
        /// <param name="pszFilter">调用方提供的指向字符串的指针，该字符串设置为计算机的设备实例标识符的子集 (ID) 或 NULL。 请参阅 ulFlags 的以下说明。</param>
        /// <param name="Buffer">用于接收一组以 NULL 结尾的设备实例标识符字符串的缓冲区的地址。 集的末尾以额外的 NULL 结尾。 应通过调用 CM_Get_Device_ID_List_Size 获取所需的缓冲区大小。</param>
        /// <param name="BufferLen">由 Buffer 指定的缓冲区的调用方提供的长度（以字符为单位）。</param>
        /// <param name="ulFlags">用于指定搜索筛选器</param>
        /// <returns>如果操作成功，函数将返回CR_SUCCESS。 否则，它将返回 Cfgmgr32.h 中定义的CR_前缀错误代码之一。</returns>
        [DllImport(CfgMgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_Device_ID_ListW", PreserveSig = true, SetLastError = false)]
        public static extern CR CM_Get_Device_ID_List([MarshalAs(UnmanagedType.LPWStr)] string pszFilter, [MarshalAs(UnmanagedType.LPArray), Out] byte[] Buffer, int BufferLen, CM_GETIDLIST_FILTER ulFlags);

        /// <summary>
        /// CM_Get_Device_ID_List_Size 函数检索保存本地计算机 设备实例的 设备实例 ID 列表所需的缓冲区大小。
        /// </summary>
        /// <param name="pulLen">接收一个值，该值表示所需的缓冲区大小（以字符为单位）。</param>
        /// <param name="pszFilter">调用方提供的指向字符串的指针，该字符串指定计算机的设备实例标识符的子集，或 NULL。 请参阅以下 ulFlags的说明。</param>
        /// <param name="ulFlags">指定搜索筛选器的可选调用方提供的位标志之一。 如果未指定任何标志，该函数会提供保存所有设备实例的所有实例标识符所需的缓冲区大小。 有关位标志的列表，请参阅 CM_Get_Device_ID_List的 ulFlags 说明。</param>
        /// <returns>如果操作成功，该函数将返回CR_SUCCESS。 否则，它将返回 Cfgmgr32.h中定义的CR_前缀错误代码之一。</returns>
        [DllImport(CfgMgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_Device_ID_List_SizeW", PreserveSig = true, SetLastError = false)]
        public static extern CR CM_Get_Device_ID_List_Size(out int pulLen, [MarshalAs(UnmanagedType.LPWStr)] string pszFilter, CM_GETIDLIST_FILTER ulFlags);

        /// <summary>
        /// CM_Get_DevNode_Property 函数检索设备实例属性。
        /// </summary>
        /// <param name="dnDevInst">绑定到本地计算机的设备实例句柄。</param>
        /// <param name="PropertyKey">指向 DEVPROPKEY 结构的指针，该结构表示所请求的设备实例属性的设备属性键。</param>
        /// <param name="PropertyType">指向 DEVPROP_TYPE 类型变量的指针，该变量接收请求的设备实例属性的 property-data-type 标识符，其中 property-data-type 标识符是基数据类型标识符与 base-data 类型修饰符（如果修改基数据类型）之间的按位 OR。</param>
        /// <param name="PropertyBuffer">指向接收请求的设备实例属性的缓冲区的指针。 仅 当缓冲区大到足以保存所有属性值数据时，CM_Get_DevNode_Property才检索请求的属性。 指针可以为 NULL。</param>
        /// <param name="PropertyBufferSize">PropertyBuffer 缓冲区的大小（以字节为单位）。 如果 PropertyBuffer 设置为 NULL， 则 *PropertyBufferSize 必须设置为零。 作为输出，如果缓冲区的大小不足以容纳所有属性值数据， CM_Get_DevNode_Property 返回 *PropertyBufferSize 中的数据大小（以字节为单位）。</param>
        /// <param name="ulFlags">保留。 必须设置为零。</param>
        /// <returns>如果操作成功，函数将返回CR_SUCCESS。 否则，它将返回 Cfgmgr32.h 中定义的CR_前缀错误代码之一。</returns>
        [DllImport(CfgMgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_DevNode_PropertyW", PreserveSig = true, SetLastError = false)]
        public static extern CR CM_Get_DevNode_Property(uint dnDevInst, ref DEVPROPKEY PropertyKey, out DEVPROP_TYPE PropertyType, IntPtr PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        /// <summary>
        /// CM_Locate_DevNode 函数获取与本地计算机上的指定 设备实例 ID 关联的设备节点的设备实例句柄。
        /// </summary>
        /// <param name="pdnDevInst">指向 CM_Locate_DevNode 检索的设备实例句柄的指针。 检索的句柄绑定到本地计算机。</param>
        /// <param name="pDeviceID">指向表示 设备实例 ID的 NULL 终止字符串的指针。 如果此值 NULL，或者如果它指向零长度字符串，则该函数将检索设备实例句柄设备树根处的设备。</param>
        /// <param name="ulFlags">标志在调用方提供设备实例标识符。</param>
        /// <returns>如果操作成功，CM_Locate_DevNode 返回CR_SUCCESS。 否则，该函数将返回 Cfgmgr32.h中定义的 CR_Xxx 错误代码之一。</returns>
        [DllImport(CfgMgr32, CharSet = CharSet.Unicode, EntryPoint = "CM_Locate_DevNodeW", PreserveSig = true, SetLastError = false)]
        public static extern CR CM_Locate_DevNode(out uint pdnDevInst, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceID, CM_LOCATE_DEVNODE_FLAGS ulFlags);
    }
}
