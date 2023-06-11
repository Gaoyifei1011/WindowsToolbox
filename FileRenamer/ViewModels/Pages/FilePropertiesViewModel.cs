using FileRenamer.Services.Root;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FileRenamer.ViewModels.Pages
{
    /// <summary>
    /// 文件属性页面视图模型
    /// </summary>
    public sealed class FilePropertiesViewModel
    {
        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        public void OnDragOver(object sender, DragEventArgs args)
        {
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = ResourceService.GetLocalized("FileProperties/DragOverContent");
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        public async void OnDrop(object sender, DragEventArgs args)
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
    }
}
