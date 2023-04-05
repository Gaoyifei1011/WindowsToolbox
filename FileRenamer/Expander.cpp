#include "pch.h"
#include "Expander.h"
#include "Expander.g.cpp"

using namespace winrt;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Composition;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Hosting;

namespace winrt::FileRenamer::implementation
{
	DependencyProperty Expander::_headerProperty = DependencyProperty::Register(
		L"Header",
		xaml_typename<IInspectable>(),
		xaml_typename<FileRenamer::Expander>(),
		PropertyMetadata{ IInspectable{nullptr} }
	);

	DependencyProperty Expander::_headerTemplateProperty = DependencyProperty::Register(
		L"HeaderTemplate",
		xaml_typename<DataTemplate>(),
		xaml_typename<FileRenamer::Expander>(),
		PropertyMetadata{ DataTemplate{nullptr} }
	);

	DependencyProperty Expander::_isExpandedProperty = DependencyProperty::Register(
		L"IsExpanded",
		xaml_typename<bool>(),
		xaml_typename<FileRenamer::Expander>(),
		PropertyMetadata{
			box_value<bool>(false),
			PropertyChangedCallback(&Expander::OnIsExpandedPropertyChanged)
		}
	);

	DependencyProperty Expander::_negativeContentHeightProperty = DependencyProperty::Register(
		L"NegativeContentHeight",
		xaml_typename<double>(),
		xaml_typename<FileRenamer::Expander>(),
		PropertyMetadata{ box_value<double>(0.0) }
	);

	Expander::Expander() {};

	IInspectable Expander::Header()
	{
		return GetValue(_headerProperty);
	}
	void Expander::Header(IInspectable const& value)
	{
		SetValue(_headerProperty, value);
	}

	DataTemplate Expander::HeaderTemplate()
	{
		return unbox_value<DataTemplate>(GetValue(_headerTemplateProperty));
	}
	void Expander::HeaderTemplate(DataTemplate const& value)
	{
		SetValue(_headerTemplateProperty, box_value(value));
	}

	bool Expander::IsExpanded()
	{
		return unbox_value<bool>(GetValue(_isExpandedProperty));
	}
	void Expander::IsExpanded(bool const& value)
	{
		SetValue(_isExpandedProperty, box_value(value));
	}

	double Expander::NegativeContentHeight()
	{
		return unbox_value<double>(GetValue(_negativeContentHeightProperty));
	}
	void Expander::NegativeContentHeight(double const& value)
	{
		SetValue(_negativeContentHeightProperty, box_value(value));
	}

	void Expander::OnApplyTemplate()
	{
		if (Border expanderContentClip = GetTemplateChild(L"ExpanderContentClip").try_as<Border>())
		{
			Visual visual = ElementCompositionPreview::GetElementVisual(expanderContentClip);
			visual.Clip(visual.Compositor().CreateInsetClip());
		}

		if (Border expanderContent = GetTemplateChild(L"ExpanderContent").try_as<Border>())
		{
			m_contentSizeChangedRevoker = expanderContent.SizeChanged(
				auto_revoke, { this,&Expander::OnContentSizeChanged });
		}

		Expander::UpdateExpandState(false);
	}

	void Expander::UpdateExpandState(bool useTransitions)
	{
		const bool isExpanded = Expander::IsExpanded();

		if (isExpanded)
		{
			VisualStateManager::GoToState(*this, L"ExpandDown", useTransitions);
		}
		else
		{
			VisualStateManager::GoToState(*this, L"CollapseUp", useTransitions);
		}
	}

	DependencyProperty Expander::HeaderProperty()
	{
		return _headerProperty;
	}

	DependencyProperty Expander::HeaderTemplateProperty()
	{
		return _headerTemplateProperty;
	}

	DependencyProperty Expander::IsExpandedProperty()
	{
		return _isExpandedProperty;
	}

	DependencyProperty Expander::NegativeContentHeightProperty()
	{
		return _negativeContentHeightProperty;
	}

	void Expander::OnIsExpandedPropertyChanged(DependencyObject const& sender, DependencyPropertyChangedEventArgs const& args)
	{
		FileRenamer::Expander owner = sender.as<FileRenamer::Expander>();
		FileRenamer::implementation::Expander* self = get_self<Expander>(owner);
		self->UpdateExpandState(true);
	}

	void Expander::OnContentSizeChanged(IInspectable const& sender, SizeChangedEventArgs const& args)
	{
		float height = args.NewSize().Height;
		Expander::NegativeContentHeight(-1.0 * height);
	}
}