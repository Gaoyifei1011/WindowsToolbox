using System;
using System.Diagnostics;
using System.Windows.Forms;
using WindowsTools.Services.Root;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        public static bool CopyToClipBoard(string content)
        {
            try
            {
                Clipboard.SetDataObject(content);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLogEntryType.Error, "Copy text to clipboard failed", e);
                return false;
            }
        }
    }
}
