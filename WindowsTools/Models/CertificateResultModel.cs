using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsTools.Models
{
    public class CertificateResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }

            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }

            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        private bool _result;

        public bool Result
        {
            get { return _result; }

            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件证书是否已经修改过
        /// </summary>
        private bool _isModified;

        public bool IsModified
        {
            get { return _isModified; }

            set
            {
                _isModified = value;
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
