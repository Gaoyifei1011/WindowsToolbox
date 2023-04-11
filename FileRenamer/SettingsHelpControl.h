#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "SettingsHelpControl.g.h"

namespace winrt::FileRenamer::implementation
{
	struct SettingsHelpControl : SettingsHelpControlT<SettingsHelpControl>
	{
	public:
		SettingsHelpControl();

		winrt::hstring SettingsHelp();

	private:
		winrt::hstring _settingsHelp;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct SettingsHelpControl : SettingsHelpControlT<SettingsHelpControl, implementation::SettingsHelpControl>
	{
	};
}
