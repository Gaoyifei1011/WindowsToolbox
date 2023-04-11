#include "pch.h"
#include "SettingsPage.h"
#include "SettingsPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	SettingsPage::SettingsPage()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::SettingsViewModel>();

		_title = AppResourcesService.GetLocalized(L"Settings/Title");
		_appearance = AppResourcesService.GetLocalized(L"Settings/Appearance");
		_general = AppResourcesService.GetLocalized(L"Settings/General");
		_advanced = AppResourcesService.GetLocalized(L"Settings/Advanced");
		_restartApp = AppResourcesService.GetLocalized(L"Settings/RestartApp");
		_restartAppToolTip = AppResourcesService.GetLocalized(L"Settings/RestartAppToolTip");
	};

	winrt::FileRenamer::SettingsViewModel SettingsPage::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring SettingsPage::Title()
	{
		return _title;
	}

	winrt::hstring SettingsPage::Appearance()
	{
		return _appearance;
	}

	winrt::hstring SettingsPage::General()
	{
		return _general;
	}

	winrt::hstring SettingsPage::Advanced()
	{
		return _advanced;
	}

	winrt::hstring SettingsPage::RestartApp()
	{
		return _restartApp;
	}

	winrt::hstring SettingsPage::RestartAppToolTip()
	{
		return _restartAppToolTip;
	}
}