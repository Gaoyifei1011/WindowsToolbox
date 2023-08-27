using FileRenamer.Strings;

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
        public static string AboutReferenceToolTipFormat(string content)
        {
            return string.Format("{0}\n{1}", content, About.ReferenceToolTip);
        }

        /// <summary>
        /// 关于界面感谢介绍内容格式化
        /// </summary>
        public static string AboutThanksToolTipFormat(string content)
        {
            return string.Format("{0}\n{1}", content, About.ThanksToolTip);
        }
    }
}
