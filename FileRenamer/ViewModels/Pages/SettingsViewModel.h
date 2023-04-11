#pragma once

#include <winrt/Windows.UI.Xaml.Controls.h>

#include "Extensions/Command/RelayCommand.h"
#include "MileWindow.h"
#include "SettingsViewModel.g.h"

namespace winrt
{
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
}

namespace winrt::FileRenamer::implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel>
	{
	public:
		SettingsViewModel();

		WinrtInput::ICommand RestartCommand();

	private:
		WinrtInput::ICommand _restartCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel, implementation::SettingsViewModel>
	{
	};
}
