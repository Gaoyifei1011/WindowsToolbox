using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.CustomControls.DialogsAndFlyouts;
using FileRenamer.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 文件名称页面视图模型
    /// </summary>
    public sealed class FileNameViewModel : ViewModelBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private string _renameRule = "<#>";

        public string RenameRule
        {
            get { return _renameRule; }

            set
            {
                _renameRule = value;
                OnPropertyChanged();
            }
        }

        private string _startNumber = "1";

        public string StartNumber
        {
            get { return _startNumber; }

            set
            {
                _startNumber = value;
                OnPropertyChanged();
            }
        }

        private string _lookUpString;

        public string LookUpString
        {
            get { return _lookUpString; }

            set
            {
                _lookUpString = value;
                OnPropertyChanged();
            }
        }

        private string _replaceString;

        public string ReplaceString
        {
            get { return _replaceString; }

            set
            {
                _replaceString = value;
                OnPropertyChanged();
            }
        }

        private NumberFormatModel _selectedNumberFormat;

        public NumberFormatModel SelectedNumberFormat
        {
            get { return _selectedNumberFormat; }

            set
            {
                _selectedNumberFormat = value;
                OnPropertyChanged();
            }
        }

        public List<NumberFormatModel> NumberFormatList { get; } = new List<NumberFormatModel>
        {
            new NumberFormatModel(){ DisplayName = ResourceService.GetLocalized("FileName/Auto"), InternalName = "Auto"},
            new NumberFormatModel(){ DisplayName = "0", InternalName = "0"},
            new NumberFormatModel(){ DisplayName = "00", InternalName = "00"},
            new NumberFormatModel(){ DisplayName = "000", InternalName = "000"},
            new NumberFormatModel(){ DisplayName = "0000", InternalName = "0000"},
            new NumberFormatModel(){ DisplayName = "00000", InternalName = "00000"},
            new NumberFormatModel(){ DisplayName = "000000", InternalName = "000000"},
            new NumberFormatModel(){ DisplayName = "0000000", InternalName = "0000000"},
        };

        public ObservableCollection<OldAndNewNameModel> FileNameDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        public FileNameViewModel()
        {
            SelectedNumberFormat = NumberFormatList[0];
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            FileNameDataList.Clear();
        }

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        public void OnDragOver(object sender, Windows.UI.Xaml.DragEventArgs args)
        {
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = ResourceService.GetLocalized("FileName/DragOverContent");
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        public async void OnDrop(object sender, Windows.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral deferral = args.GetDeferral();
            try
            {
                DataPackageView view = args.DataView;
                if (view.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> filesList = await view.GetStorageItemsAsync();
                    foreach (IStorageItem item in filesList)
                    {
                        FileNameDataList.Add(new OldAndNewNameModel() { OriginalFileName = item.Name });
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        /// <summary>
        /// 选择编号格式
        /// </summary>
        public void OnNumberFormatClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedNumberFormat = NumberFormatList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        public void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = ResourceService.GetLocalized("FileName/SelectFolder");
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                }
            }
        }

        /// <summary>
        /// 显示改名示例窗口
        /// </summary>
        public void OnViewNameChangeExampleClicked(object sender, RoutedEventArgs args)
        {
            NameChangeForm nameChangeForm = new NameChangeForm();
            nameChangeForm.Show();
        }
    }
}
