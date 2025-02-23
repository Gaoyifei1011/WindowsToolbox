namespace WindowsTools.WindowsAPI.PInvoke.CfgMgr32
{
    /// <summary>
    /// 指定搜索筛选器
    /// </summary>
    public enum CM_GETIDLIST_FILTER
    {
        /// <summary>
        /// 如果设置了此标志，将忽略 pszFilter ，并返回系统上所有设备的列表。
        /// </summary>
        CM_GETIDLIST_FILTER_NONE = 0x00000000,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定设备枚举器的名称，（可选）后跟 设备 ID。 字符串格式为 EnumeratorName\<DeviceID>，例如 ROOT 或 ROOT\*PNP0500。
        /// 如果 pszFilter 仅提供枚举器名称，则函数将返回与枚举器关联的每个设备的实例的设备实例 ID 。 可以通过调用 CM_Enumerate_Enumerators 来获取枚举器名称。
        /// 如果 pszFilter 同时提供枚举器和 设备 ID，则函数仅返回与枚举器关联的指定设备的实例的设备实例 ID 。
        /// </summary>
        CM_GETIDLIST_FILTER_ENUMERATOR = 0x00000001,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定 Microsoft Windows 服务的名称， (通常是驱动程序) 。 函数返回由指定服务控制的设备实例的设备实例 ID。
        /// 请注意，如果设备树不包含指定服务的 开发节点 ，则此函数默认创建一个。 若要禁止此行为，还需设置 CM_GETIDLIST_DONOTGENERATE。
        /// 如果未指定搜索筛选器标志，则函数将返回所有设备实例的所有设备实例 ID。
        /// </summary>
        CM_GETIDLIST_FILTER_SERVICE = 0x00000002,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定设备实例标识符。 函数返回指定设备实例的 弹出关系的 设备实例 ID。
        /// </summary>
        CM_GETIDLIST_FILTER_EJECTRELATIONS = 0x00000004,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定设备实例标识符。 函数返回指定设备实例的 删除关系的 设备实例 ID。
        /// </summary>
        CM_GETIDLIST_FILTER_REMOVALRELATIONS = 0x00000008,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定设备实例标识符。 函数返回指定设备实例的电源关系的设备实例 ID。
        /// </summary>
        CM_GETIDLIST_FILTER_POWERRELATIONS = 0x00000010,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定设备实例标识符。 函数返回指定设备实例的 总线关系的 设备实例 ID。
        /// </summary>
        CM_GETIDLIST_FILTER_BUSRELATIONS = 0x00000020,

        /// <summary>
        /// 仅用于 CM_GETIDLIST_FILTER_SERVICE。 如果已设置，并且设备树不包含指定服务的 devnode，则此标志将阻止函数为服务创建开发节点。
        /// </summary>
        CM_GETIDLIST_DONOTGENERATE = 0x10000040,

        /// <summary>
        /// 如果设置了此标志， pszFilter 必须指定复合设备节点的设备实例标识符 (devnode) 。
        /// 该函数返回表示指定复合开发节点的传输关系的开发节点的设备实例标识符。
        /// </summary>
        CM_GETIDLIST_FILTER_TRANSPORTRELATIONS = 0x00000080,

        /// <summary>
        /// 如果设置了此标志，则返回的列表仅包含系统上当前存在的设备实例。 此值可以与其他 ulFlags 值（如 CM_GETIDLIST_FILTER_CLASS）组合使用。
        /// </summary>
        CM_GETIDLIST_FILTER_PRESENT = 0x00000100,

        /// <summary>
        /// 如果设置了此标志， pszFilter 将包含一个指定 设备安装类 GUID 的字符串。 返回的列表包含 (CM_DRP_CLASSGUID常量引用的属性) 与指定的设备安装类 GUID 匹配的设备实例。
        /// CM_DRP_CLASSGUID常量在 Cfgmgr32.h 中定义。
        /// </summary>
        CM_GETIDLIST_FILTER_CLASS = 0x00000200,

        CM_GETIDLIST_FILTER_BITS = 0x100003FF,
    }
}
