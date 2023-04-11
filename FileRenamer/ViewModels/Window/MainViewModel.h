#pragma once

#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.Xaml.Interop.h>

#include "Extensions/Command/RelayCommand.h"
#include "MainViewModel.g.h"
#include "WinMain.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtData = Windows::UI::Xaml::Data;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtInterop = Windows::UI::Xaml::Interop;
	namespace WinrtNavigation = Windows::UI::Xaml::Navigation;
	namespace WinrtXaml = Windows::UI::Xaml;
}

namespace winrt::FileRenamer::implementation
{
	struct MainViewModel : MainViewModelT<MainViewModel>
	{
	public:
		MainViewModel();

		bool IsBackEnabled();
		void IsBackEnabled(bool const& value);

		winrt::WinrtControls::NavigationViewItem SelectedItem();
		void SelectedItem(winrt::WinrtControls::NavigationViewItem const& value);

		winrt::WinrtInput::ICommand NavigationItemCommand();
		winrt::WinrtInput::ICommand ClickCommand();

		winrt::WinrtCollections::IMap<hstring, winrt::WinrtInterop::TypeName> PageDict();

		void OnNavigationViewBackRequested(winrt::WinrtControls::NavigationView const& sender, winrt::WinrtControls::NavigationViewBackRequestedEventArgs const& args);
		void OnNavigationViewLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args);
		void OnFrameNavigated(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtNavigation::NavigationEventArgs const& args);
		void OnFrameNavgationFailed(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtNavigation::NavigationFailedEventArgs const& args);

		event_token PropertyChanged(winrt::WinrtData::PropertyChangedEventHandler const& value);
		void PropertyChanged(event_token const& token) noexcept;

	private:
		bool _isBackEnabled;
		winrt::WinrtControls::NavigationViewItem _selectedItem;

		winrt::WinrtInput::ICommand _navigationItemCommand;
		winrt::WinrtInput::ICommand _clickCommand;

		winrt::WinrtCollections::IMap<hstring, winrt::WinrtInterop::TypeName> _pageDict{ winrt::single_threaded_map<hstring, winrt::WinrtInterop::TypeName>() };

		winrt::event<winrt::WinrtData::PropertyChangedEventHandler> m_propertyChanged;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainViewModel : MainViewModelT<MainViewModel, implementation::MainViewModel>
	{
	};
}
