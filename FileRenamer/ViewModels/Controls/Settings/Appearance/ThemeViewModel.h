#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.System.h>

#include "Extensions/Command/RelayCommand.h"
#include "ThemeViewModel.g.h"

namespace winrt
{
    namespace WinrtFoundation = Windows::Foundation;
    namespace WinrtInput = Windows::UI::Xaml::Input;
    namespace WinrtSystem = Windows::System;
}

namespace winrt::FileRenamer::implementation
{
    struct ThemeViewModel : ThemeViewModelT<ThemeViewModel>
    {
    public:
        ThemeViewModel();

        winrt::WinrtInput::ICommand SettingsColorCommand();

    private:
        winrt::WinrtInput::ICommand _settingsColorCommand;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ThemeViewModel : ThemeViewModelT<ThemeViewModel, implementation::ThemeViewModel>
    {
    };
}
