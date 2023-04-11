﻿#pragma once

#include "pch.h"
#include "MileWindow.h"
#include "SettingsViewModel.h"
#include "SettingsViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	SettingsViewModel::SettingsViewModel()
	{
		_restartCommand = winrt::make<RelayCommand>([this](winrt::WinrtFoundation::IInspectable parameter) -> WinrtFoundation::IAsyncAction
			{
				FileRenamer::RestartAppsDialog dialog;
				dialog.XamlRoot(MileWindow::Current()->Content().XamlRoot());
				co_await dialog.ShowAsync();
			});
	};

	/// <summary>
	/// 打开重启应用确认的窗口对话框
	/// </summary>
	WinrtInput::ICommand SettingsViewModel::RestartCommand()
	{
		return _restartCommand;
	}
}