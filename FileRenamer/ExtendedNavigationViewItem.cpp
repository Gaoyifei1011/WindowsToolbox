#pragma once

#include "pch.h"
#include "ExtendedNavigationViewItem.h"
#include "ExtendedNavigationViewItem.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls::Primitives;

namespace winrt::FileRenamer::implementation
{
	DependencyProperty ExtendedNavigationViewItem::_commandProperty = DependencyProperty::Register(
		L"Command",
		xaml_typename<ICommand>(),
		xaml_typename<FileRenamer::ExtendedNavigationViewItem>(),
		PropertyMetadata{ nullptr }
	);

	DependencyProperty ExtendedNavigationViewItem::_commandParameterProperty = DependencyProperty::Register(
		L"CommandParameter",
		xaml_typename<IInspectable>(),
		xaml_typename<FileRenamer::ExtendedNavigationViewItem>(),
		PropertyMetadata{ nullptr }
	);

	DependencyProperty ExtendedNavigationViewItem::_toolTipProperty = DependencyProperty::Register(
		L"ToolTip",
		xaml_typename<hstring>(),
		xaml_typename<FileRenamer::ExtendedNavigationViewItem>(),
		PropertyMetadata{ nullptr }
	);

	ExtendedNavigationViewItem::ExtendedNavigationViewItem()
	{
		InitializeComponent();

		this->Loaded({ this,&ExtendedNavigationViewItem::OnLoaded });
		this->Tapped({ this,&ExtendedNavigationViewItem::OnItemTapped });
	};

	ICommand ExtendedNavigationViewItem::Command()
	{
		return unbox_value<ICommand>(GetValue(_commandProperty));
	}
	void ExtendedNavigationViewItem::Command(ICommand const& value)
	{
		SetValue(_commandProperty, box_value(value));
	}

	DependencyProperty ExtendedNavigationViewItem::CommandProperty()
	{
		return _commandProperty;
	}

	IInspectable ExtendedNavigationViewItem::CommandParameter()
	{
		return unbox_value<IInspectable>(GetValue(_commandParameterProperty));
	}
	void ExtendedNavigationViewItem::CommandParameter(IInspectable const& value)
	{
		SetValue(_commandParameterProperty, box_value(value));
	}

	DependencyProperty ExtendedNavigationViewItem::CommandParameterProperty()
	{
		return _commandParameterProperty;
	}

	hstring ExtendedNavigationViewItem::ToolTip()
	{
		return unbox_value<hstring>(GetValue(_toolTipProperty));
	}
	void ExtendedNavigationViewItem::ToolTip(hstring const& value)
	{
		SetValue(_toolTipProperty, box_value(value));
	}

	DependencyProperty ExtendedNavigationViewItem::ToolTipProperty()
	{
		return _toolTipProperty;
	}

	void ExtendedNavigationViewItem::OnLoaded(IInspectable const& sender, RoutedEventArgs const& args)
	{
		if (ExtendedNavigationViewItem::ToolTip() != L"")
		{
			Controls::ToolTip NavigationViewItemToolTip;
			NavigationViewItemToolTip.Content(box_value(ExtendedNavigationViewItem::ToolTip() + L" "));
			NavigationViewItemToolTip.Placement(PlacementMode::Bottom);
			NavigationViewItemToolTip.VerticalOffset(20);
			Controls::ToolTipService::SetToolTip(this->try_as<DependencyObject>(), box_value(NavigationViewItemToolTip));
		}
	}

	/// <summary>
	/// 点击导航控件项时触发命令
	/// </summary>
	void ExtendedNavigationViewItem::OnItemTapped(IInspectable const& sender, TappedRoutedEventArgs const& args)
	{
		ICommand clickCommand = ExtendedNavigationViewItem::Command();
		if (clickCommand != nullptr)
		{
			clickCommand.Execute(ExtendedNavigationViewItem::CommandParameter());
		}
	}
}