using System;
using WindowsTools.Strings;

namespace WindowsTools.Helpers.Converters
{
    /// <summary>
    /// 字符串格式化辅助类
    /// </summary>
    public static class StringConverterHelper
    {
        /// <summary>
        /// 关于界面项目引用内容格式化
        /// </summary>
        public static string AboutReferenceToolTipFormat(object content)
        {
            return string.Format("{0}{1}{2}", content, Environment.NewLine, About.ReferenceToolTip);
        }

        /// <summary>
        /// 关于界面感谢介绍内容格式化
        /// </summary>
        public static string AboutThanksToolTipFormat(object content)
        {
            return string.Format("{0}{1}{2}", content, Environment.NewLine, About.ThanksToolTip);
        }

        /// <summary>
        /// 复选框状态文字提示
        /// </summary>
        public static string CheckBoxToolTipFormat(bool isSelected, string content)
        {
            if (isSelected)
            {
                if (content.Equals("PriExtract", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Format(PriExtract.SelectedToolTip, content);
                }
                else
                {
                    return string.Format(PriExtract.UnSelectedToolTip, content);
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
