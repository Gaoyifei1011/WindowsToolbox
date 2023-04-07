#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "SettingsHelpControl.g.h"

namespace winrt::FileRenamer::implementation
{
    struct SettingsHelpControl : SettingsHelpControlT<SettingsHelpControl>
    {
    public:
        SettingsHelpControl();

        hstring SettingsHelp();

    private:
        hstring _settingsHelp;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct SettingsHelpControl : SettingsHelpControlT<SettingsHelpControl, implementation::SettingsHelpControl>
    {
    };
}
