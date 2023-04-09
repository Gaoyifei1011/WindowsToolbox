#pragma once

#include "winrt/Windows.UI.Xaml.Controls.h"
#include "pch.h"
#include "MileWindow.h"
#include "SettingsViewModel.h"
#include "SettingsViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::FileRenamer::implementation
{
	SettingsViewModel::SettingsViewModel()
	{
		_restartCommand = make<RelayCommand>([this](IInspectable parameter) -> IAsyncAction
			{
				FileRenamer::RestartAppsDialog dialog;
				dialog.XamlRoot(MileWindow::Current()->Content().XamlRoot());
				co_await dialog.ShowAsync();
			});
	};

	/// <summary>
	/// 打开重启应用确认的窗口对话框
	/// </summary>
	ICommand SettingsViewModel::RestartCommand()
	{
		return _restartCommand;
	}
}