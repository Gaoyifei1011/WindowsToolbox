#pragma once

#include "winrt/Windows.UI.Xaml.Hosting.h"
#include "pch.h"
#include "Expander.g.h"

using namespace winrt;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Composition;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Hosting;

namespace winrt::FileRenamer::implementation
{
	struct Expander : ExpanderT<Expander>
	{
		Expander();

		IInspectable Header();
		void Header(IInspectable const& value);

		DataTemplate HeaderTemplate();
		void HeaderTemplate(DataTemplate const& value);

		bool IsExpanded();
		void IsExpanded(bool const& value);

		double NegativeContentHeight();
		void NegativeContentHeight(double const& value);

		void OnApplyTemplate();
		void UpdateExpandState(bool useTransitions);

		static DependencyProperty HeaderProperty();
		static DependencyProperty HeaderTemplateProperty();
		static DependencyProperty IsExpandedProperty();
		static DependencyProperty NegativeContentHeightProperty();

		static void OnIsExpandedPropertyChanged(DependencyObject const& sender, DependencyPropertyChangedEventArgs const& args);

	private:
		static DependencyProperty _headerProperty;
		static DependencyProperty _headerTemplateProperty;
		static DependencyProperty _isExpandedProperty;
		static DependencyProperty _negativeContentHeightProperty;

		void OnContentSizeChanged(IInspectable const& sender, SizeChangedEventArgs const& args);
		Border::SizeChanged_revoker m_contentSizeChangedRevoker{};
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct Expander : ExpanderT<Expander, implementation::Expander>
	{
	};
}
