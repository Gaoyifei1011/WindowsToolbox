using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WUApiLib;

namespace WindowsTools.Models
{
    public class UpdateModel : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string KBNumber { get; set; }

        public string Description { get; set; }

        public string SupportURL { get; set; }

        public string UpdateID { get; set; }

        public DateTime Date { get; set; }

        public string HResult { get; set; }

        public int EventID { get; set; }

        public string ELDescription { get; set; }

        public string ELProvider { get; set; }

        public DateTime ELDate { get; set; }

        private OperationResultCode _resultCode;

        public OperationResultCode ResultCode
        {
            get { return _resultCode; }

            set
            {
                _resultCode = value;
                OnPropertyChanged();
            }
        }

        private tagUpdateOperation _tagUpdateOperation;

        public tagUpdateOperation TagUpdateOperation
        {
            get { return _tagUpdateOperation; }

            set
            {
                _tagUpdateOperation = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
