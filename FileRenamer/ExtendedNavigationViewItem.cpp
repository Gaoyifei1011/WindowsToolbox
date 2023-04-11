#include "pch.h"
#include "ExtendedNavigationViewItem.h"
#include "ExtendedNavigationViewItem.g.cpp"

namespace winrt::FileRenamer::implementation
{
	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::_commandProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"Command",
		xaml_typename<winrt::WinrtInput::ICommand>(),
		xaml_typename<winrt::FileRenamer::ExtendedNavigationViewItem>(),
		winrt::WinrtXaml::PropertyMetadata{ nullptr }
	);

	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::_commandParameterProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"CommandParameter",
		xaml_typename<winrt::WinrtFoundation::IInspectable>(),
		xaml_typename<winrt::FileRenamer::ExtendedNavigationViewItem>(),
		winrt::WinrtXaml::PropertyMetadata{ nullptr }
	);

	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::_toolTipProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"ToolTip",
		xaml_typename<winrt::hstring>(),
		xaml_typename<winrt::FileRenamer::ExtendedNavigationViewItem>(),
		winrt::WinrtXaml::PropertyMetadata{ nullptr }
	);

	ExtendedNavigationViewItem::ExtendedNavigationViewItem()
	{
		InitializeComponent();

		this->Loaded({ this,&ExtendedNavigationViewItem::OnLoaded });
		this->Tapped({ this,&ExtendedNavigationViewItem::OnItemTapped });
	};

	winrt::WinrtInput::ICommand ExtendedNavigationViewItem::Command()
	{
		return winrt::unbox_value<winrt::WinrtInput::ICommand>(GetValue(_commandProperty));
	}
	void ExtendedNavigationViewItem::Command(winrt::WinrtInput::ICommand const& value)
	{
		SetValue(_commandProperty, winrt::box_value(value));
	}

	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::CommandProperty()
	{
		return _commandProperty;
	}

	winrt::WinrtFoundation::IInspectable ExtendedNavigationViewItem::CommandParameter()
	{
		return winrt::unbox_value<winrt::WinrtFoundation::IInspectable>(GetValue(_commandParameterProperty));
	}
	void ExtendedNavigationViewItem::CommandParameter(winrt::WinrtFoundation::IInspectable const& value)
	{
		SetValue(_commandParameterProperty, winrt::box_value(value));
	}

	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::CommandParameterProperty()
	{
		return _commandParameterProperty;
	}

	winrt::hstring ExtendedNavigationViewItem::ToolTip()
	{
		return winrt::unbox_value<winrt::hstring>(GetValue(_toolTipProperty));
	}
	void ExtendedNavigationViewItem::ToolTip(winrt::hstring const& value)
	{
		SetValue(_toolTipProperty, winrt::box_value(value));
	}

	winrt::WinrtXaml::DependencyProperty ExtendedNavigationViewItem::ToolTipProperty()
	{
		return _toolTipProperty;
	}

	void ExtendedNavigationViewItem::OnLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args)
	{
		if (ExtendedNavigationViewItem::ToolTip() != L"")
		{
			winrt::WinrtControls::ToolTip NavigationViewItemToolTip;
			NavigationViewItemToolTip.Content(winrt::box_value(ExtendedNavigationViewItem::ToolTip() + L" "));
			NavigationViewItemToolTip.Placement(winrt::WinrtPrimitives::PlacementMode::Bottom);
			NavigationViewItemToolTip.VerticalOffset(20);
			winrt::WinrtControls::ToolTipService::SetToolTip(this->try_as<winrt::WinrtXaml::DependencyObject>(), winrt::box_value(NavigationViewItemToolTip));
		}
	}

	/// <summary>
	/// 点击导航控件项时触发命令
	/// </summary>
	void ExtendedNavigationViewItem::OnItemTapped(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtInput::TappedRoutedEventArgs const& args)
	{
		winrt::WinrtInput::ICommand clickCommand = ExtendedNavigationViewItem::Command();
		if (clickCommand != nullptr)
		{
			clickCommand.Execute(ExtendedNavigationViewItem::CommandParameter());
		}
	}
}