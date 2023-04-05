#pragma once

#include "Extensions/Command/RelayCommand.h"
#include "SettingsViewModel.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel>
	{
	public:
		SettingsViewModel();

		ICommand RestartCommand();

	private:
		ICommand _restartCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel, implementation::SettingsViewModel>
	{
	};
}
