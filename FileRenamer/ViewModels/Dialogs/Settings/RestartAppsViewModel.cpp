#include "pch.h"
#include "RestartAppsViewModel.h"
#include "RestartAppsViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::FileRenamer::implementation
{
	void RestartApps();

	RestartAppsViewModel::RestartAppsViewModel()
	{
		_restartAppsCommand = make<RelayCommand>([this](IInspectable dialog)
			{
				if (dialog != nullptr)
				{
					ContentDialog restartAppsDialog = unbox_value<ContentDialog>(dialog);
					restartAppsDialog.Hide();
					RestartApps();
				}
			});

		_closeDialogCommand = make<RelayCommand>([this](IInspectable dialog)
			{
				if (dialog != nullptr)
				{
					ContentDialog restartAppsDialog = unbox_value<ContentDialog>(dialog);
					restartAppsDialog.Hide();
				}
			});
	};

	/// <summary>
	/// 重启应用
	/// </summary>
	ICommand RestartAppsViewModel::RestartAppsCommand()
	{
		return _restartAppsCommand;
	}

	/// <summary>
	/// 取消重启应用
	/// </summary>
	ICommand RestartAppsViewModel::CloseDialogCommand()
	{
		return _closeDialogCommand;
	}

	/// <summary>
	/// 重启应用
	/// </summary>
	void RestartApps()
	{

	}
}
