using FileRenamer.Models;
using FileRenamer.Services.Root;
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
    /// 扩展名称页面视图模型
    /// </summary>
    public sealed class ExtensionNameViewModel
    {
        public ObservableCollection<OldAndNewNameModel> ExtensionNameDataList { get; } = new ObservableCollection<OldAndNewNameModel>();

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            ExtensionNameDataList.Clear();
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
            args.DragUIOverride.Caption = ResourceService.GetLocalized("ExtensionName/DragOverContent");
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
                        ExtensionNameDataList.Add(new OldAndNewNameModel() { OriginalFileName = item.Name });
                    }
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
    }
}
