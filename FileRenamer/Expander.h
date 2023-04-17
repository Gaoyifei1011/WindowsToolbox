#pragma once

#include <winrt/Windows.UI.Xaml.Hosting.h>

#include "pch.h"
#include "Expander.g.h"

namespace winrt
{
	namespace WinrtComposition = Windows::UI::Composition;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtHosting = Windows::UI::Xaml::Hosting;
	namespace WinrtXaml = Windows::UI::Xaml;
}

namespace winrt::FileRenamer::implementation
{
	struct Expander : ExpanderT<Expander>
	{
		Expander();

		winrt::WinrtFoundation::IInspectable Header();
		void Header(winrt::WinrtFoundation::IInspectable const& value);

		winrt::WinrtXaml::DataTemplate HeaderTemplate();
		void HeaderTemplate(winrt::WinrtXaml::DataTemplate const& value);

		bool IsExpanded();
		void IsExpanded(bool const& value);

		double NegativeContentHeight();
		void NegativeContentHeight(double const& value);

		void OnApplyTemplate();
		void UpdateExpandState(bool useTransitions);

		static winrt::WinrtXaml::DependencyProperty HeaderProperty();
		static winrt::WinrtXaml::DependencyProperty HeaderTemplateProperty();
		static winrt::WinrtXaml::DependencyProperty IsExpandedProperty();
		static winrt::WinrtXaml::DependencyProperty NegativeContentHeightProperty();

		static void OnIsExpandedPropertyChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args);

	private:
		static winrt::WinrtXaml::DependencyProperty _headerProperty;
		static winrt::WinrtXaml::DependencyProperty _headerTemplateProperty;
		static winrt::WinrtXaml::DependencyProperty _isExpandedProperty;
		static winrt::WinrtXaml::DependencyProperty _negativeContentHeightProperty;

		void OnContentSizeChanged(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::SizeChangedEventArgs const& args);
		winrt::WinrtControls::Border::SizeChanged_revoker m_contentSizeChangedRevoker{};
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct Expander : ExpanderT<Expander, implementation::Expander>
	{
	};
}
