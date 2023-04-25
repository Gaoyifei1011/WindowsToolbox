#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.Xaml.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Interop.h>

#include "AdaptiveHeightValueConverter.g.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInterop = Windows::UI::Xaml::Interop;
	namespace WinrtXaml = Windows::UI::Xaml;
}

namespace winrt::FileRenamer::implementation
{
	struct AdaptiveHeightValueConverter : AdaptiveHeightValueConverterT<AdaptiveHeightValueConverter>
	{
	public:
		AdaptiveHeightValueConverter();

		winrt::WinrtXaml::Thickness DefaultItemMargin();
		void DefaultItemMargin(winrt::WinrtXaml::Thickness const& value);

		winrt::WinrtFoundation::IInspectable Convert(
			winrt::WinrtFoundation::IInspectable const& value,
			winrt::WinrtInterop::TypeName const& targetType,
			winrt::WinrtFoundation::IInspectable const& parameter,
			winrt::hstring const& language
		);

		winrt::WinrtFoundation::IInspectable ConvertBack(
			winrt::WinrtFoundation::IInspectable const& value,
			winrt::WinrtInterop::TypeName const& targetType,
			winrt::WinrtFoundation::IInspectable const& parameter,
			winrt::hstring const& language
		);

		static winrt::WinrtXaml::Thickness GetItemMargin(winrt::WinrtControls::GridView const& view, winrt::WinrtXaml::Thickness const& fallback = { 0,0,0,0 });

	private:
		winrt::WinrtXaml::Thickness _defaultItemMargin;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AdaptiveHeightValueConverter : AdaptiveHeightValueConverterT<AdaptiveHeightValueConverter, implementation::AdaptiveHeightValueConverter>
	{
	};
}
