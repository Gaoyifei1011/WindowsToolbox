#include "pch.h"
#include "ThemeViewModel.h"
#include "ThemeViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThemeViewModel::ThemeViewModel()
	{
		_settingsColorCommand = make<RelayCommand>([this](winrt::WinrtFoundation::IInspectable parameter) -> winrt::WinrtFoundation::IAsyncAction
			{
				winrt::WinrtFoundation::Uri uri(L"ms-settings:colors");
				co_await winrt::WinrtSystem::Launcher::LaunchUriAsync(uri);
			});
	}

	winrt::WinrtInput::ICommand ThemeViewModel::SettingsColorCommand()
	{
		return _settingsColorCommand;
	}
}