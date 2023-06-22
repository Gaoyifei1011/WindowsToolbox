using FileRenamer.Models;
using FileRenamer.Services.Root;
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
    /// 大写小写页面视图模型
    /// </summary>
    public sealed class UpperAndLowerCasePageViewModel
    {
        public ObservableCollection<OldAndNewNameModel> UpperAndLowerCaseDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            UpperAndLowerCaseDataList.Clear();
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("UpperAndLowerCase/DragOverContent");
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
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        public void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = ResourceService.GetLocalized("UpperAndLowerCase/SelectFolder");
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
    }
}
