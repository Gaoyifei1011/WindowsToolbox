using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsTools.Models;
using WindowsTools.Strings;
using WUApiLib;

namespace WindowsTools.Views.Pages
{
    /// <summary>
    /// Windows 更新管理页面
    /// </summary>
    public sealed partial class UpdateManagerPage : Page
    {
        private UpdateSession updateSession = new UpdateSession();

        private ObservableCollection<UpdateModel> UpdateList = new ObservableCollection<UpdateModel>();

        public UpdateManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：Windows 更新管理页面——挂载的事件

        /// <summary>
        /// 打开 Windows 更新
        /// </summary>
        private void OnWindowsUpdateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:windowsupdate");
            });
        }

        /// <summary>
        /// 打开预览体验计划设置
        /// </summary>
        private void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("ms-settings:windowsinsider");
            });
        }

        #endregion 第一部分：Windows 更新管理页面——挂载的事件

        private void InitializeUpdate()
        {
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            int count = updateSearcher.GetTotalHistoryCount();

            foreach (IUpdateHistoryEntry updateHistoryEntry in updateSearcher.QueryHistory(0, count))
            {
                Match match = Regex.Match(updateHistoryEntry.Title, @"\b(KB\d+)\b");
                UpdateModel updateItem = new UpdateModel()
                {
                    Title = updateHistoryEntry.Title,
                    KBNumber = match.Success ? match.Groups[1].Value : UpdateManager.Unknown,
                    Date = updateHistoryEntry.Date.ToLocalTime(),
                    ResultCode = updateHistoryEntry.ResultCode,
                    HResult = updateHistoryEntry.HResult.ToString(),
                    TagUpdateOperation = updateHistoryEntry.Operation,
                    UpdateID = updateHistoryEntry.UpdateIdentity.UpdateID,
                    Description = updateHistoryEntry.Description,
                    SupportURL = updateHistoryEntry.SupportUrl,
                    ELDescription = ""
                };
            }
        }
    }
}
