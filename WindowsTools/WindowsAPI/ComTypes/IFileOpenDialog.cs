using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// 通过添加特定于打开对话框的方法扩展 IFileOpenDialog 接口。
    /// </summary>
    [ComImport, Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOpenDialog
    {
        /// <summary>
        /// 使用模态窗口显示对话框
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>

        [PreserveSig]
        int Show([In] IntPtr hwnd);

        /// <summary>
        /// 设置对话框可以打开或保存的文件类型。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        /// </summary>
        [PreserveSig]
        int SetFileTypes();

        /// <summary>
        /// 设置在对话框中显示为所选的文件类型。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileTypeIndex();

        /// <summary>
        /// 获取当前选定的文件类型。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</return>
        [PreserveSig]
        int GetFileTypeIndex();

        /// <summary>
        /// 分配侦听来自对话的事件的事件处理程序。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Advise();

        /// <summary>
        /// 删除通过 Advise 方法附加的事件处理程序。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Unadvise();

        /// <summary>
        /// 设置标志以控制对话的行为。
        /// </summary>
        /// <param name="fos">FILEOPENDIALOGOPTIONS 值的一个或多个。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOptions([In] FILEOPENDIALOGOPTIONS fos);

        /// <summary>
        /// 获取设置为控制对话框行为的当前标志。
        /// </summary>
        /// <param name="pfos">此方法成功返回时，指向由一个或多个 FILEOPENDIALOGOPTIONS 值组成的值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetOptions(out FILEOPENDIALOGOPTIONS pfos);

        /// <summary>
        /// 如果最近使用的文件夹值不可用，则设置用作默认值的文件夹。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetDefaultFolder();

        /// <summary>
        /// 设置在打开对话框时始终选择的文件夹，而不考虑以前的用户操作。
        /// </summary>
        /// <param name="psi">指向表示文件夹的接口的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFolder(IShellItem psi);

        /// <summary>
        /// 获取当前在对话框中选择的文件夹，或者，如果当前未显示对话框，则打开对话框时要选择的文件夹。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFolder();

        /// <summary>
        /// 获取对话框中用户的当前选择。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCurrentSelection();

        /// <summary>
        /// 设置打开该对话框时显示在“ 文件名 编辑”框中的文件名。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileName();

        /// <summary>
        /// 检索对话框的 “文件名 编辑”框中当前输入的文本。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFileName();

        /// <summary>
        /// 设置打开该对话框时显示在 “文件名 编辑”框中的文件名。
        /// </summary>
        /// <param name="pszName">指向文件名称的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// 设置 “打开 ”或 “保存 ”按钮的文本。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOkButtonLabel();

        /// <summary>
        /// 设置文件名编辑框旁边的标签文本。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFileNameLabel();

        /// <summary>
        /// 获取用户在对话框中所做的选择。
        /// </summary>
        /// <param name="ppsi">指向表示用户选择的 IShellItem 的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetResult(out IShellItem ppsi);

        /// <summary>
        /// 将文件夹添加到可供用户打开或保存项的位置列表中。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int AddPlace();

        /// <summary>
        /// 设置要添加到文件名的默认扩展名。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetDefaultExtension();

        /// <summary>
        /// 关闭对话框。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Close();

        /// <summary>
        /// 使调用应用程序能够将 GUID 与对话框的持久状态相关联。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetClientGuid();

        /// <summary>
        /// 指示对话框清除所有持久状态信息。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ClearClientData();

        /// <summary>
        /// 设置筛选器。
        /// 已弃用。 SetFilter 不再可用于 Windows 7。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetFilter();

        /// <summary>
        /// 在允许多项选择的对话框中获取用户的选择。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetResults();

        /// <summary>
        /// 获取对话框中当前选定的项。这些项目可能是在视图中选择的项目，也可能是在文件名编辑框中选择的文本。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSelectedItems();
    }
}
