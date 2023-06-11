﻿using FileRenamer.Models;
using FileRenamer.Services.Root;
using FileRenamer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<FileNameModel> FileNameDataList { get; } = new ObservableCollection<FileNameModel>();

        /// <summary>
        /// 清空列表
        /// </summary>
        public void OnClearList(object sender, RoutedEventArgs args)
        {
            FileNameDataList.Clear();
        }

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        public void OnDragOver(object sender, DragEventArgs args)
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
        public async void OnDrop(object sender, DragEventArgs args)
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
                        FileNameDataList.Add(new FileNameModel() { OriginalFileName = item.Name });
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
