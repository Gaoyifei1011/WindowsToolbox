using FileRenamer.Properties;
using FileRenamer.ViewModels.Base;
using System.Text;

namespace FileRenamer.ViewModels.Dialogs
{
    /// <summary>
    /// 应用许可证对话框视图模型
    /// </summary>
    public sealed class LicenseViewModel : ViewModelBase
    {
        private string _licenseText;

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                _licenseText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 加载许可证信息
        /// </summary>
        public void OnLoading(object sender, object args)
        {
            LicenseText = Encoding.UTF8.GetString(Resources.LICENSE);
        }
    }
}
