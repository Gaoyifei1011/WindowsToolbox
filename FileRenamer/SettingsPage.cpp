#pragma once

#include "pch.h"
#include "SettingsPage.h"
#include "SettingsPage.g.cpp"
#include "WinMain.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	SettingsPage::SettingsPage()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::SettingsViewModel>();

		_title = AppResourcesService.GetLocalized(L"Settings/Title");
		_restartApp = AppResourcesService.GetLocalized(L"Settings/RestartApp");
		_restartAppToolTip = AppResourcesService.GetLocalized(L"Settings/RestartAppToolTip");
	};

	FileRenamer::SettingsViewModel SettingsPage::ViewModel()
	{
		return _viewModel;
	}

	hstring SettingsPage::Title()
	{
		return _title;
	}

	hstring SettingsPage::RestartApp()
	{
		return _restartApp;
	}

	hstring SettingsPage::RestartAppToolTip()
	{
		return _restartAppToolTip;
	}
}