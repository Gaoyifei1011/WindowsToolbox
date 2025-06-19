using Microsoft.UI.Xaml.Controls;
using PowerTools.Extensions.DataType.Enums;
using PowerTools.Services.Root;
using Windows.UI.Xaml;

namespace PowerTools.Views.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip
    {
        public OperationResultTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.GenerateTextEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("GenerateTextEmpty");
            }
            else if (operationKind is OperationKind.GenerateBarCodeFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("GenerateBarCodeFailed");
            }
            else if (operationKind is OperationKind.GenerateQRCodeFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("GenerateQRCodeFailed");
            }
            else if (operationKind is OperationKind.ParsePhotoFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ParsePhotoFailed");
            }
            else if (operationKind is OperationKind.ReadClipboardImageFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ReadClipboardImageFailed");
            }
            else if (operationKind is OperationKind.DeleteFileFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("FileDeleteFailed");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("LanguageChange");
            }
            else if (operationKind is OperationKind.ListEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ListEmpty");
            }
            else if (operationKind is OperationKind.TextEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("TextEmpty");
            }
            else if (operationKind is OperationKind.ThemeChangeSameTime)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ThemeChangeSameTime");
            }
            else if (operationKind is OperationKind.ThemeSwitchSaveResult)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ThemeSwitchSaveResult");
            }
            else if (operationKind is OperationKind.ThemeSwitchRestoreResult)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ThemeSwitchRestoreResult");
            }
            else if (operationKind is OperationKind.MenuTitleEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuTitleEmpty");
            }
            else if (operationKind is OperationKind.MenuDefaultIconPathEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuDefaultIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuLightThemeIconPathEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuLightThemeIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuDarkThemeIconPathEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuDarkThemeIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuProgramPathEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuProgramPathEmpty");
            }
            else if (operationKind is OperationKind.MenuMatchRuleEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("MenuMatchRuleEmpty");
            }
            else if (operationKind is OperationKind.NoOperation)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("NoOperation");
            }
            else if (operationKind is OperationKind.ShellMenuNeedToRefreshData)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ShellMenuNeedToRefreshData");
            }
            else if (operationKind is OperationKind.InsiderPreviewSettings)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("InsiderPreviewSuccessfully");
            }
            else if (operationKind is OperationKind.AddDriverAllSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("AddDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.AddDriverPartialSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("AddDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.AddDriverFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("AddDriverFailed");
            }
            else if (operationKind is OperationKind.AddInstallDriverAllSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("AddInstallDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.AddInstallDriverPartialSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("AddInstallDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.AddInstallDriverFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("AddInstallDriverFailed");
            }
            else if (operationKind is OperationKind.DeleteDriverAllSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("DeleteDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteDriverPartialSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("DeleteDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteDriverSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("DeleteDriverSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteDriverFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("DeleteDriverFailed");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverAllSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ForceDeleteDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverPartialSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ForceDeleteDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverSuccessfully)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ForceDeleteDriverSuccessfully");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverFailed)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ForceDeleteDriverFailed");
            }
            else if (operationKind is OperationKind.SelectDriverEmpty)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.NotificationResource.GetString("SelectDriverEmpty");
            }
        }

        public OperationResultTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("NewestVersion");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("NotNewestVersion");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("LogCleanSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("LogCleanFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.LoopbackSetResult)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("LoopbackSetResultSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("LoopbackSetResultFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("TerminateProcessSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("TerminateProcessFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.ContextMenuUpdate)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("ContextMenuUpdateSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("ContextMenuUpdateFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.CleanUpdateHistory)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.NotificationResource.GetString("CleanUpdateHistorySuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.NotificationResource.GetString("CleanUpdateHistoryFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
        }

        public OperationResultTip(OperationKind operationKind, int successItems, int failedItems)
        {
            InitializeComponent();

            if (operationKind is OperationKind.File)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = failedItems is 0 ? string.Format(ResourceService.NotificationResource.GetString("FileResultSuccessfully"), successItems) : string.Format(ResourceService.NotificationResource.GetString("FileResultFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.IconExtract)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = failedItems is 0 ? string.Format(ResourceService.NotificationResource.GetString("IconExtractSuccessfully"), successItems) : string.Format(ResourceService.NotificationResource.GetString("IconExtractFailed"), successItems, failedItems);
            }
        }
    }
}
