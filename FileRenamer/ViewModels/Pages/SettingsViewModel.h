#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.System.h>

#include "Extensions/Command/RelayCommand.h"
#include "MileWindow.h"
#include "Models/Settings/Appearence/ThemeModel.h"
#include "Services/Controls/Settings/Appearance/ThemeService.h"
#include "SettingsViewModel.g.h"

namespace winrt
{
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtSystem = Windows::System;
}

namespace winrt::FileRenamer::implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel>
	{
	public:
		SettingsViewModel();

		winrt::FileRenamer::ThemeModel Theme();
		void Theme(winrt::FileRenamer::ThemeModel const& value);

		winrt::WinrtInput::ICommand RestartCommand();
		winrt::WinrtInput::ICommand SettingsColorCommand();
		winrt::WinrtInput::ICommand ThemeSelectCommand();

	private:
		winrt::FileRenamer::ThemeModel _theme{ nullptr };

		winrt::WinrtInput::ICommand _restartCommand;
		winrt::WinrtInput::ICommand _settingsColorCommand;
		winrt::WinrtInput::ICommand _themeSelectCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsViewModel : SettingsViewModelT<SettingsViewModel, implementation::SettingsViewModel>
	{
	};
}
