namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 为 ExecuteRequested 事件提供事件数据。
    /// </summary>
    public class ExecuteRequestedEventArgs(object parameter)
    {
        public object Parameter { get; } = parameter;
    }
}
