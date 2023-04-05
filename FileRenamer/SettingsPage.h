#pragma once

#include "winrt/base.h"
#include "SettingsPage.g.h"
#include "ViewModels/Pages/SettingsViewModel.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage>
	{
	public:
		SettingsPage();
		FileRenamer::SettingsViewModel ViewModel();

		hstring Title();
		hstring RestartApp();
		hstring RestartAppToolTip();

	private:
		FileRenamer::SettingsViewModel _viewModel;

		hstring _title;
		hstring _restartApp;
		hstring _restartAppToolTip;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsPage : SettingsPageT<SettingsPage, implementation::SettingsPage>
	{
	};
}
