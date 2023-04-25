#pragma once

#include <winrt/base.h>

#include "global.h"
#include "SettingsPage.g.h"
#include "ViewModels/Pages/SettingsViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage>
	{
	public:
		SettingsPage();
		winrt::FileRenamer::SettingsViewModel ViewModel();

		winrt::hstring Advanced();
		winrt::hstring Appearance();
		winrt::hstring General();
		winrt::hstring RestartApp();
		winrt::hstring RestartAppToolTip();
		winrt::hstring ThemeMode();
		winrt::hstring ThemeModeDescription();
		winrt::hstring Title();
		winrt::hstring WindowsColorSettings();

	private:
		winrt::FileRenamer::SettingsViewModel _viewModel;

		winrt::hstring _advanced;
		winrt::hstring _appearance;
		winrt::hstring _general;
		winrt::hstring _restartApp;
		winrt::hstring _restartAppToolTip;
		winrt::hstring _themeMode;
		winrt::hstring _themeModeDescription;
		winrt::hstring _title;
		winrt::hstring _windowsColorSettings;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage, implementation::SettingsPage>
	{
	};
}
