#pragma once

#include "MainWindowViewModel.g.h"

namespace winrt::FileRenamer::implementation
{
    struct MainWindowViewModel : MainWindowViewModelT<MainWindowViewModel>
    {
        MainWindowViewModel();

        int32_t MyProperty();
        void MyProperty(int32_t const& value);

    private:
        int32_t myProperty;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct MainWindowViewModel : MainWindowViewModelT<MainWindowViewModel, implementation::MainWindowViewModel>
    {
    };
}
