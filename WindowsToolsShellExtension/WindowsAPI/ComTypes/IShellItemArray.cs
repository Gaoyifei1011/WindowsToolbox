using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WindowsToolsShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开用于创建和操作 Shell 项 数组的方法。
    /// </summary>
    [GeneratedComInterface, Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
    public partial interface IShellItemArray
    {
        /// <summary>
        /// 通过指定的处理程序绑定到对象。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 IBindCtx 接口的指针。</param>
        /// <param name="bhid">以下值之一，在 Shlguid.h 中定义，用于确定处理程序。</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppvOut">此方法返回时，包含 riid 中指定的对象，该对象由 rbhid 指定的处理程序返回。</param>
        [PreserveSig]
        int BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppvOut);

        /// <summary>
        /// 获取属性存储。
        /// </summary>
        /// <param name="flags">GETPROPERTYSTOREFLAGS 常量之一。</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppv">此方法返回时，包含 riid 中请求的接口指针。 这通常是 IPropertyStore 或 IPropertyStoreCapabilities。</param>
        [PreserveSig]
        int GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// 获取 shell 项数组中项的属性说明列表。
        /// </summary>
        /// <param name="keyType">对 PROPERTYKEY 结构的引用，指定要检索的属性列表。</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppv">此方法返回时，包含 riid 中请求的接口。 这通常是 IPropertyDescriptionList。</param>
        [PreserveSig]
        int GetPropertyDescriptionList(IntPtr keyType, ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// 获取 IShellItemArray 中包含的项集的属性。 如果数组包含多个项，则此方法检索的属性不是单个项的属性，而是所有项的所有请求属性的逻辑组合。
        /// </summary>
        /// <param name="dwAttribFlags">如果数组包含单个项，则此方法提供与 GetAttributes 相同的结果。 但是，如果数组包含多个项，则所有项的属性集将合并为单个属性集，并在 psfgaoAttribs 指向的值中返回。</param>
        /// <param name="sfgaoMask">一个掩码，用于指定要请求的特定属性。 一个或多个 SFGAO 值的按位 OR。</param>
        /// <param name="psfgaoAttribs">此方法成功返回时包含所请求属性的值的位图。</param>
        [PreserveSig]
        int GetAttributes(int dwAttribFlags, uint sfgaoMask, out uint psfgaoAttribs);

        /// <summary>
        /// 获取给定 IShellItem 数组中的项数。
        /// </summary>
        /// <param name="pdwNumItems">此方法返回时，包含 IShellItemArray 中的项数。</param>
        [PreserveSig]
        int GetCount(out uint pdwNumItems);

        /// <summary>
        /// 获取 IShellItemArray 中给定索引处的项。
        /// </summary>
        /// <param name="dwIndex">IShellItemArray 中请求的 IShellItem 的索引</param>
        /// <param name="ppsi">此方法返回时，包含请求的 IShellItem 指针。</param>
        [PreserveSig]
        int GetItemAt(uint dwIndex, out IShellItem ppsi);

        /// <summary>
        /// 获取数组中项的枚举器。
        /// </summary>
        /// <param name="ppenumShellItems">此方法返回时，包含一个 IEnumShellItems 指针，该指针枚举数组中的 shell 项 。</param>
        [PreserveSig]
        int EnumItems(out IntPtr ppenumShellItems);
    }
}
