﻿using System.ComponentModel;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.UI.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    public sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _licenseText;

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                _licenseText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LicenseText)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LicenseDialog()
        {
            InitializeComponent();
            LicenseText = Encoding.UTF8.GetString(Properties.Resources.LICENSE);
        }
    }
}
