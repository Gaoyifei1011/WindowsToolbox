#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "SettingsPage.g.h"
#include "ViewModels/Pages/SettingsViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage>
	{
	public:
		SettingsPage();
		winrt::FileRenamer::SettingsViewModel ViewModel();

		winrt::hstring Title();
		winrt::hstring Appearance();
		winrt::hstring General();
		winrt::hstring Advanced();
		winrt::hstring RestartApp();
		winrt::hstring RestartAppToolTip();

	private:
		winrt::FileRenamer::SettingsViewModel _viewModel;

		winrt::hstring _title;
		winrt::hstring _appearance;
		winrt::hstring _general;
		winrt::hstring _advanced;
		winrt::hstring _restartApp;
		winrt::hstring _restartAppToolTip;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage, implementation::SettingsPage>
	{
	};
}
