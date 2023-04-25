#include "pch.h"
#include "SettingsPage.h"
#include "SettingsPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	SettingsPage::SettingsPage()
	{
		InitializeComponent();

		_viewModel = winrt::make<FileRenamer::implementation::SettingsViewModel>();

		_advanced = AppResourceService.GetLocalized(L"Settings/Advanced");
		_appearance = AppResourceService.GetLocalized(L"Settings/Appearance");
		_general = AppResourceService.GetLocalized(L"Settings/General");
		_restartApp = AppResourceService.GetLocalized(L"Settings/RestartApp");
		_restartAppToolTip = AppResourceService.GetLocalized(L"Settings/RestartAppToolTip");
		_themeMode = AppResourceService.GetLocalized(L"Settings/ThemeMode");
		_themeModeDescription = AppResourceService.GetLocalized(L"Settings/ThemeModeDescription");
		_title = AppResourceService.GetLocalized(L"Settings/Title");
		_windowsColorSettings = AppResourceService.GetLocalized(L"Settings/WindowsColorSettings");
	};

	winrt::FileRenamer::SettingsViewModel SettingsPage::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring SettingsPage::Advanced()
	{
		return _advanced;
	}

	winrt::hstring SettingsPage::Appearance()
	{
		return _appearance;
	}

	winrt::hstring SettingsPage::General()
	{
		return _general;
	}

	winrt::hstring SettingsPage::RestartApp()
	{
		return _restartApp;
	}

	winrt::hstring SettingsPage::RestartAppToolTip()
	{
		return _restartAppToolTip;
	}

	winrt::hstring SettingsPage::ThemeMode()
	{
		return _themeMode;
	}

	winrt::hstring SettingsPage::ThemeModeDescription()
	{
		return _themeModeDescription;
	}

	winrt::hstring SettingsPage::Title()
	{
		return _title;
	}

	winrt::hstring SettingsPage::WindowsColorSettings()
	{
		return _windowsColorSettings;
	}
}