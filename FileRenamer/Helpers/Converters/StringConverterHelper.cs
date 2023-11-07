using FileRenamer.Strings;
using System;

namespace FileRenamer.Helpers.Converters
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
    }
}
