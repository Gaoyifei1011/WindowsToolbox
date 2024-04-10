using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WindowsToolsShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开用于检索有关 Shell 项的信息的方法。 IShellItem 和 IShellItem2 是任何新代码中项的首选表示形式。
    /// </summary>
    [GeneratedComInterface, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IShellItem
    {
        /// <summary>
        /// 绑定到由 BHID) (处理程序 ID 值指定的项的处理程序。
        /// </summary>
        /// <param name="pbc">指向绑定上下文对象上的 IBindCtx 接口的指针。 用于将可选参数传递给处理程序。 绑定上下文的内容特定于处理程序。 例如，绑定到 BHID_Stream时，绑定上下文中的 STGM 标志指示所需的访问模式 (读取或读/写) 。</param>
        /// <param name="bhid">对 GUID 的引用，该 GUID 指定要创建哪个处理程序</param>
        /// <param name="riid">要检索的对象类型的 IID。</param>
        /// <param name="ppv">此方法返回时，包含由 rbhid 指定的处理程序返回的 riid 类型的指针。</param>
        void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// 获取 IShellItem 对象的父对象。
        /// </summary>
        /// <param name="ppsi">指向 IShellItem 接口父级的指针的地址。</param>
        void GetParent(out IShellItem ppsi);

        /// <summary>
        /// 获取 IShellItem 对象的显示名称。
        /// </summary>
        /// <param name="sigdnName">指示名称外观的 SIGDN 值之一。</param>
        /// <param name="ppszName">一个值，当此函数成功返回时，接收指向检索到的显示名称的指针的地址。</param>
        void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

        /// <summary>
        /// 获取 IShellItem 对象的一组请求的属性。
        /// </summary>
        /// <param name="sfgaoMask">指定要检索的属性。 一个或多个 SFGAO 值。 使用按位 OR 运算符确定要检索的属性。</param>
        /// <param name="psfgaoAttribs">指向一个值的指针，当此方法成功返回时，该值包含请求的属性。 一个或多个 SFGAO 值。 仅返回 由 sfgaoMask 指定的那些属性;其他属性值未定义。</param>
        void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

        /// <summary>
        /// 比较两个 IShellItem 对象。
        /// </summary>
        /// <param name="psi">指向 IShellItem 对象的指针，用于与现有 IShellItem 对象进行比较。</param>
        /// <param name="hint">确定如何执行比较的 SICHINTF 值之一。 有关此参数的可能值列表，请参阅 SICHINTF 。</param>
        /// <param name="piOrder">此参数接收比较结果。 如果两个项相同，则此参数等于零;如果它们不同，则参数为非零。</param>
        void Compare(IShellItem psi, uint hint, out int piOrder);
    }
}
