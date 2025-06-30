using System;
using System.Windows.Input;

// 抑制 CS0067 警告
#pragma warning disable CS0067

namespace PowerToolbox.Extensions.DataType.Class
{
    public class ExecuteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public event EventHandler<ExecuteRequestedEventArgs> ExecuteRequested;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteRequested?.Invoke(this, new ExecuteRequestedEventArgs(parameter));
        }
    }
}
