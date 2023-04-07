#include "pch.h"
#include "SettingsHelpControl.h"
#include "SettingsHelpControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	SettingsHelpControl::SettingsHelpControl()
	{
		InitializeComponent();

		_settingsHelp = AppResourcesService.GetLocalized(L"About/SettingsHelp");
	}

	hstring SettingsHelpControl::SettingsHelp()
	{
		return _settingsHelp;
	}
}
