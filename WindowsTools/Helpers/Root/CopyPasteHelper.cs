using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public static bool CopyToClipboard(string content)
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

        /// <summary>
        /// 读取剪贴板中的图片
        /// </summary>
        public static Image ReadClipboardImage()
        {
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData.GetDataPresent(DataFormats.MetafilePict))
                {
                    return Clipboard.GetImage();
                }
                else if (iData.GetDataPresent(DataFormats.FileDrop))
                {
                    StringCollection files = Clipboard.GetFileDropList();
                    if (files.Count is 0)
                    {
                        return null;
                    }
                    else
                    {
                        return Image.FromFile(files[0]);
                    }
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    string path = (string)iData.GetData(DataFormats.Text);
                    char[] chars = Path.GetInvalidPathChars();
                    if (path.IndexOfAny(chars) >= 0)
                    {
                        return null;
                    }
                    if (File.Exists(path))
                    {
                        return Image.FromFile(path);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLogEntryType.Error, "Read clipboard failed", e);
                return null;
            }
        }
    }
}
