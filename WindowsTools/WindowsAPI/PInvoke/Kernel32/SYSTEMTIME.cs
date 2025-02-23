using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTools.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定日期和时间，使用月份、日、年、工作日、小时、分钟、秒和毫秒的单个成员。 时间采用协调世界时 (UTC) 或本地时间，具体取决于正在调用的函数。
    /// </summary>
    public struct SYSTEMTIME
    {
        /// <summary>
        /// 年。 此成员的有效值为 1601 到 30827。
        /// </summary>
        public ushort Year;

        /// <summary>
        /// 月份。
        /// </summary>
        public ushort Month;

        /// <summary>
        /// 星期几。
        /// </summary>
        public ushort DayOfWeek;

        /// <summary>
        /// 每月的日期。 此成员的有效值为 1 到 31。
        /// </summary>
        public ushort Day;

        /// <summary>
        /// 小时。 此成员的有效值为 0 到 23。
        /// </summary>
        public ushort Hour;

        /// <summary>
        /// 分钟。 此成员的有效值为 0 到 59。
        /// </summary>
        public ushort Minute;

        /// <summary>
        /// 秒钟。 此成员的有效值为 0 到 59。
        /// </summary>
        public ushort Second;

        /// <summary>
        /// 毫秒。 此成员的有效值为 0 到 999。
        /// </summary>
        public ushort Milliseconds;
    }
}
