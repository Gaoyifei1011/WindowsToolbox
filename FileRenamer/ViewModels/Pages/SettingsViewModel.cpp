#pragma once

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

		_settingsColorCommand = make<RelayCommand>([this](winrt::WinrtFoundation::IInspectable parameter) -> winrt::WinrtFoundation::IAsyncAction
			{
				winrt::WinrtFoundation::Uri uri(L"ms-settings:colors");
				co_await winrt::WinrtSystem::Launcher::LaunchUriAsync(uri);
			});
	};

	winrt::FileRenamer::ThemeModel SettingsViewModel::Theme()
	{
		return _theme;
	}
	void SettingsViewModel::Theme(winrt::FileRenamer::ThemeModel const& value)
	{
		_theme = value;
	}

	/// <summary>
	/// 打开重启应用确认的窗口对话框
	/// </summary>
	winrt::WinrtInput::ICommand SettingsViewModel::RestartCommand()
	{
		return _restartCommand;
	}

	/// <summary>
	/// 打开系统主题设置
	/// </summary>
	winrt::WinrtInput::ICommand SettingsViewModel::SettingsColorCommand()
	{
		return _settingsColorCommand;
	}

	/// <summary>
	/// 主题修改设置
	/// </summary>
	/// <returns></returns>
	winrt::WinrtInput::ICommand SettingsViewModel::ThemeSelectCommand()
	{
		return _themeSelectCommand;
	}
}