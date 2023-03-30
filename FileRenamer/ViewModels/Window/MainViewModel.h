#pragma once

#include "winrt/Windows.Foundation.Collections.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "Extensions/Command/RelayCommand.h"
#include "MainViewModel.g.h"
#include "WinMain.h"

using namespace winrt;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Data;
using namespace winrt::Windows::UI::Xaml::Interop;
using namespace winrt::Windows::UI::Xaml::Navigation;

namespace winrt::FileRenamer::implementation
{
    struct MainViewModel : MainViewModelT<MainViewModel>
    {
    public:
        MainViewModel();

        bool IsBackEnabled();
        void IsBackEnabled(bool const& value);

        IInspectable SelectedItem();
        void SelectedItem(IInspectable const& value);

        ICommand NavigationItemCommand();
        ICommand ClickCommand();

        Collections::IMap<hstring,TypeName> PageDict();
        
        void OnNavigationViewBackRequested(NavigationView const& sender, NavigationViewBackRequestedEventArgs const& args);
        void OnNavigationViewLoaded(IInspectable const& sender, RoutedEventArgs const& args);
        void OnFrameNavigated(IInspectable const& sender, NavigationEventArgs const& args);
        void OnFrameNavgationFailed(IInspectable const& sender, NavigationFailedEventArgs const& args);

        event_token PropertyChanged(PropertyChangedEventHandler const& value);
        void PropertyChanged(event_token const& token) noexcept;

    private:
        bool _isBackEnabled;
        IInspectable _selectedItem;

        ICommand _navigationItemCommand;
        ICommand _clickCommand;

        Collections::IMap<hstring, TypeName> _pageDict{ single_threaded_map<hstring,TypeName>() };

        winrt::event<PropertyChangedEventHandler> m_propertyChanged;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct MainViewModel : MainViewModelT<MainViewModel, implementation::MainViewModel>
    {
    };
}
