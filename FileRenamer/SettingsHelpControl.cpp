#include "pch.h"
#include "SettingsHelpControl.h"
#include "SettingsHelpControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	SettingsHelpControl::SettingsHelpControl()
	{
		InitializeComponent();

		_settingsHelp = AppResourcesService.GetLocalized(L"About/SettingsHelp");
	}

	winrt::hstring SettingsHelpControl::SettingsHelp()
	{
		return _settingsHelp;
	}
}