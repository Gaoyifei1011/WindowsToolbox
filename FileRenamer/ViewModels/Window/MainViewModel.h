#pragma once

#include "Extensions/Command/RelayCommand.h"
#include "MainViewModel.g.h"
#include "WinMain.h"

namespace winrt::FileRenamer::implementation
{
    struct MainViewModel : MainViewModelT<MainViewModel>
    {
        MainViewModel();

        ICommand ClickCommand();

    private:
        ICommand _clickCommand;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct MainViewModel : MainViewModelT<MainViewModel, implementation::MainViewModel>
    {
    };
}
