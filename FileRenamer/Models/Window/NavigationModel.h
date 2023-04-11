#pragma once

#include <winrt/base.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Interop.h>

#include "NavigationModel.g.h"

namespace winrt
{
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtInterop = Windows::UI::Xaml::Interop;
}

namespace winrt::FileRenamer::implementation
{
	struct NavigationModel : NavigationModelT<NavigationModel>
	{
	public:
		NavigationModel();

		winrt::hstring NavigationTag();
		void NavigationTag(winrt::hstring const& value);

		winrt::WinrtControls::NavigationViewItem NavigationItem();
		void NavigationItem(winrt::WinrtControls::NavigationViewItem const& value);

		winrt::WinrtInterop::TypeName NavigationPage();
		void NavigationPage(winrt::WinrtInterop::TypeName const& value);

	private:
		winrt::hstring _navigationTag;
		winrt::WinrtControls::NavigationViewItem _navigationItem;
		winrt::WinrtInterop::TypeName _navigationPage;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct NavigationModel : NavigationModelT<NavigationModel, implementation::NavigationModel>
	{
	};
}
