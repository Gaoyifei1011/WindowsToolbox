#pragma once

#include <WinMain.h>

#include "ThemeControl.g.h"
#include "ViewModels/Controls/Settings/Appearance/ThemeViewModel.h"

namespace winrt::FileRenamer::implementation
{
    struct ThemeControl : ThemeControlT<ThemeControl>
    {
    public:
        ThemeControl();

        winrt::FileRenamer::ThemeViewModel ViewModel();

        winrt::hstring ThemeMode();
        winrt::hstring ThemeModeDescription();
        winrt::hstring WindowsColorSettings();

    private:
        winrt::FileRenamer::ThemeViewModel _viewModel;

        winrt::hstring _themeMode;
        winrt::hstring _themeModeDescription;
        winrt::hstring _windowsColorSettings;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ThemeControl : ThemeControlT<ThemeControl, implementation::ThemeControl>
    {
    };
}
