using System;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 请求的服务。
    /// </summary>
    [Flags]
    public enum TRACKMOUSEEVENT_FLAGS : uint
    {
        /// <summary>
        /// 调用方希望取消先前的跟踪请求。 调用方还应指定要取消的跟踪类型。 例如，若要取消悬停跟踪，调用方必须传递 TME_CANCEL 和 TME_HOVER 标志。
        /// </summary>
        TME_CANCEL = 0x80000000,

        /// <summary>
        /// 调用方需要悬停通知。 通知作为 WM_MOUSEHOVER 消息传递。
        /// 如果调用方请求悬停跟踪，而悬停跟踪已处于活动状态，则将重置悬停计时器。
        /// 如果鼠标指针不在指定的窗口或区域上，则忽略此标志。
        /// </summary>
        TME_HOVER = 0x00000001,

        /// <summary>
        /// 调用方想要离开通知。 通知作为 WM_MOUSELEAVE 消息传递。 如果鼠标未在指定的窗口或区域上，则会立即生成离开通知，并且不会执行进一步的跟踪。
        /// </summary>
        TME_LEAVE = 0x00000002,

        /// <summary>
        /// 调用方希望悬停并保留非工作区的通知。 通知以 WM_NCMOUSEHOVER 和 WM_NCMOUSELEAVE 消息的形式传递。
        /// </summary>
        TME_NONCLIENT = 0x00000010,

        /// <summary>
        /// 函数填充 结构，而不是将其视为跟踪请求。 结构已填充，如果结构已传递到 TrackMouseEvent，它将生成当前跟踪。 唯一的异常是，如果在原始 TrackMouseEvent 请求期间指定了HOVER_DEFAULT，则返回的悬停超时始终是实际超时，而不是HOVER_DEFAULT。
        /// </summary>
        TME_QUERY = 0x40000000,
    }
}
