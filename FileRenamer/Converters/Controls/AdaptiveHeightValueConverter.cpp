#include "pch.h"
#include "AdaptiveHeightValueConverter.h"
#include "AdaptiveHeightValueConverter.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AdaptiveHeightValueConverter::AdaptiveHeightValueConverter()
	{
		_defaultItemMargin = winrt::WinrtXaml::Thickness{ 0,0,4,4 };
	}

	winrt::WinrtXaml::Thickness AdaptiveHeightValueConverter::DefaultItemMargin()
	{
		return _defaultItemMargin;
	}
	void AdaptiveHeightValueConverter::DefaultItemMargin(winrt::WinrtXaml::Thickness const& value)
	{
		_defaultItemMargin = value;
	}

	winrt::WinrtFoundation::IInspectable AdaptiveHeightValueConverter::Convert(winrt::WinrtFoundation::IInspectable const& value, winrt::WinrtInterop::TypeName const& targetType, winrt::WinrtFoundation::IInspectable const& parameter, winrt::hstring const& language)
	{
		if (value != nullptr)
		{
			winrt::WinrtControls::GridView gridView = parameter.try_as<winrt::WinrtControls::GridView>();
			if (gridView == nullptr)
			{
				return value;
			}

			double height = winrt::unbox_value<double>(value);

			winrt::WinrtXaml::Thickness padding = gridView.Padding();
			winrt::WinrtXaml::Thickness margin = AdaptiveHeightValueConverter::GetItemMargin(gridView, AdaptiveHeightValueConverter::DefaultItemMargin());
			height = height + margin.Top + margin.Bottom + padding.Top + padding.Bottom;

			return winrt::box_value(height);
		}

		return winrt::box_value(winrt::box_value(NAN));
	}

	winrt::WinrtFoundation::IInspectable AdaptiveHeightValueConverter::ConvertBack(winrt::WinrtFoundation::IInspectable const& value, winrt::WinrtInterop::TypeName const& targetType, winrt::WinrtFoundation::IInspectable const& parameter, winrt::hstring const& language)
	{
		return winrt::WinrtFoundation::IInspectable();
	}

	winrt::WinrtXaml::Thickness AdaptiveHeightValueConverter::GetItemMargin(winrt::WinrtControls::GridView const& view, winrt::WinrtXaml::Thickness const& fallback)
	{
		winrt::WinrtXaml::Style containerStyle = view.ItemContainerStyle();
		if (containerStyle == nullptr || containerStyle.Setters() == nullptr)
		{
			return fallback;
		}
		else
		{
			winrt::WinrtCollections::IVectorView setters = containerStyle.Setters().GetView();
			auto setterPtr = std::find_if(begin(setters), end(setters), [](winrt::WinrtXaml::SetterBase const& item)
				{
					return item.as<winrt::WinrtXaml::Setter>().Property() == winrt::WinrtXaml::FrameworkElement::MarginProperty();
				});

			if (*setterPtr != nullptr)
			{
				winrt::WinrtXaml::SetterBase first = *setterPtr;
				winrt::WinrtXaml::Setter setter = first.as<winrt::WinrtXaml::Setter>();
				winrt::WinrtFoundation::IInspectable setterVal = setter.Value();
				winrt::WinrtXaml::Thickness setterThickness = winrt::unbox_value<winrt::WinrtXaml::Thickness>(setterVal);
				return setterThickness;
			}
			else if (view.Items().Size() > 0)
			{
				auto container = view.ContainerFromIndex(0).as<winrt::WinrtControls::GridViewItem>();
				if (container != nullptr)
				{
					return container.Margin();
				}
				else
				{
					return fallback;
				}
			}
			else
			{
				return fallback;
			}
		}
	}
}