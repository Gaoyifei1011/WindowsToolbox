using System;
using System.ComponentModel;
using System.Threading;
using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Models
{
    /// <summary>
    /// 右键菜单ID项
    /// </summary>
    public class ContextMenuItemModel : INotifyPropertyChanged
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public Guid Clsid { get; set; }

        public string DllPath { get; set; }

        public ApartmentState ThreadingMode { get; set; }

        public BlockedClsidType BlockedClsidType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
