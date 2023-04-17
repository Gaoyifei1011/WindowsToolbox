#include "pch.h"
#include "Expander.h"
#include "Expander.g.cpp"

namespace winrt::FileRenamer::implementation
{
	winrt::WinrtXaml::DependencyProperty Expander::_headerProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"Header",
		winrt::xaml_typename<winrt::WinrtFoundation::IInspectable>(),
		winrt::xaml_typename<winrt::FileRenamer::Expander>(),
		winrt::WinrtXaml::PropertyMetadata{ winrt::WinrtFoundation::IInspectable{nullptr} }
	);

	winrt::WinrtXaml::DependencyProperty Expander::_headerTemplateProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"HeaderTemplate",
		winrt::xaml_typename<winrt::WinrtXaml::DataTemplate>(),
		winrt::xaml_typename<winrt::FileRenamer::Expander>(),
		winrt::WinrtXaml::PropertyMetadata{ winrt::WinrtXaml::DataTemplate{nullptr} }
	);

	winrt::WinrtXaml::DependencyProperty Expander::_isExpandedProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"IsExpanded",
		winrt::xaml_typename<bool>(),
		winrt::xaml_typename<winrt::FileRenamer::Expander>(),
		winrt::WinrtXaml::PropertyMetadata{
			winrt::box_value<bool>(false),
			winrt::WinrtXaml::PropertyChangedCallback(&Expander::OnIsExpandedPropertyChanged)
		}
	);

	winrt::WinrtXaml::DependencyProperty Expander::_negativeContentHeightProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"NegativeContentHeight",
		winrt::xaml_typename<double>(),
		winrt::xaml_typename<winrt::FileRenamer::Expander>(),
		winrt::WinrtXaml::PropertyMetadata{ winrt::box_value<double>(0.0) }
	);

	Expander::Expander() {};

	winrt::WinrtFoundation::IInspectable Expander::Header()
	{
		return GetValue(_headerProperty);
	}
	void Expander::Header(winrt::WinrtFoundation::IInspectable const& value)
	{
		SetValue(_headerProperty, value);
	}

	winrt::WinrtXaml::DataTemplate Expander::HeaderTemplate()
	{
		return winrt::unbox_value<winrt::WinrtXaml::DataTemplate>(GetValue(_headerTemplateProperty));
	}
	void Expander::HeaderTemplate(winrt::WinrtXaml::DataTemplate const& value)
	{
		SetValue(_headerTemplateProperty, winrt::box_value(value));
	}

	bool Expander::IsExpanded()
	{
		return winrt::unbox_value<bool>(GetValue(_isExpandedProperty));
	}
	void Expander::IsExpanded(bool const& value)
	{
		SetValue(_isExpandedProperty, winrt::box_value(value));
	}

	double Expander::NegativeContentHeight()
	{
		return winrt::unbox_value<double>(GetValue(_negativeContentHeightProperty));
	}
	void Expander::NegativeContentHeight(double const& value)
	{
		SetValue(_negativeContentHeightProperty, winrt::box_value(value));
	}

	void Expander::OnApplyTemplate()
	{
		if (winrt::WinrtControls::Border expanderContentClip = GetTemplateChild(L"ExpanderContentClip").try_as<winrt::WinrtControls::Border>())
		{
			winrt::WinrtComposition::Visual visual = winrt::WinrtHosting::ElementCompositionPreview::GetElementVisual(expanderContentClip);
			visual.Clip(visual.Compositor().CreateInsetClip());
		}

		if (winrt::WinrtControls::Border expanderContent = GetTemplateChild(L"ExpanderContent").try_as<winrt::WinrtControls::Border>())
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
			winrt::WinrtXaml::VisualStateManager::GoToState(*this, L"ExpandDown", useTransitions);
		}
		else
		{
			winrt::WinrtXaml::VisualStateManager::GoToState(*this, L"CollapseUp", useTransitions);
		}
	}

	winrt::WinrtXaml::DependencyProperty Expander::HeaderProperty()
	{
		return _headerProperty;
	}

	winrt::WinrtXaml::DependencyProperty Expander::HeaderTemplateProperty()
	{
		return _headerTemplateProperty;
	}

	winrt::WinrtXaml::DependencyProperty Expander::IsExpandedProperty()
	{
		return _isExpandedProperty;
	}

	winrt::WinrtXaml::DependencyProperty Expander::NegativeContentHeightProperty()
	{
		return _negativeContentHeightProperty;
	}

	void Expander::OnIsExpandedPropertyChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args)
	{
		winrt::FileRenamer::Expander owner = d.as<winrt::FileRenamer::Expander>();
		winrt::FileRenamer::implementation::Expander* self = winrt::get_self<winrt::FileRenamer::implementation::Expander>(owner);
		self->UpdateExpandState(true);
	}

	void Expander::OnContentSizeChanged(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::SizeChangedEventArgs const& args)
	{
		float height = args.NewSize().Height;
		Expander::NegativeContentHeight(-1.0 * height);
	}
}