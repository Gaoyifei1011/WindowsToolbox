using System;
using System.Windows.Forms;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        /// <param name="content">复制到剪贴板的内容</param>
        public static bool CopyToClipBoard(string content)
        {
            try
            {
                Clipboard.SetDataObject(content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
