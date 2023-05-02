using FileRenamer.Contracts;
using FileRenamer.Extensions.Command;
using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Models.Controls.Settings.Appearance;
using FileRenamer.Services.Controls.Settings.Appearance;
using FileRenamer.Services.Window;
using FileRenamer.ViewModels.Base;
using FileRenamer.Views.Pages;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace FileRenamer.ViewModels.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：界面语言设置用户控件视图模型
    /// </summary>
    public sealed class LanguageViewModel : ViewModelBase
    {
        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        private LanguageModel _language = LanguageService.AppLanguage;

        public LanguageModel Language
        {
            get { return _language; }

            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        // 语言设置说明
        public IRelayCommand LanguageTipCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        });

        // 应用默认语言修改
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                Language = args.AddedItems[0] as LanguageModel;
                await LanguageService.SetLanguageAsync(Language);
                LanguageService.SetAppLanguage(Language);
                //new LanguageChangeNotification(true).Show();
            }
        }
    }
}
