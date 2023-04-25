#pragma once

#include <winrt/Windows.UI.Xaml.h>
#include <winrt/Windows.UI.Xaml.Markup.h>
#include <winrt/Windows.UI.Xaml.Interop.h>
#include <winrt/Windows.UI.Xaml.Controls.Primitives.h>
#include <winrt/Windows.UI.Xaml.Input.h>

#include "ExtendedNavigationViewItem.g.h"

namespace winrt
{
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtPrimitives = Windows::UI::Xaml::Controls::Primitives;
	namespace WinrtXaml = Windows::UI::Xaml;
}

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	/// 扩展后的导航控件项，附带命令绑定
	/// </summary>
	struct ExtendedNavigationViewItem : ExtendedNavigationViewItemT<ExtendedNavigationViewItem>
	{
	public:
		ExtendedNavigationViewItem();

		winrt::WinrtInput::ICommand Command();
		void Command(winrt::WinrtInput::ICommand const& value);

		winrt::WinrtFoundation::IInspectable CommandParameter();
		void CommandParameter(winrt::WinrtFoundation::IInspectable const& value);

		winrt::hstring ToolTip();
		void ToolTip(hstring const& value);

		static winrt::WinrtXaml::DependencyProperty CommandProperty();
		static winrt::WinrtXaml::DependencyProperty CommandParameterProperty();
		static winrt::WinrtXaml::DependencyProperty ToolTipProperty();

		void OnTapped(winrt::WinrtInput::TappedRoutedEventArgs const& args);

	private:
		winrt::WinrtInput::ICommand _command;
		winrt::WinrtFoundation::IInspectable _commandParameter;
		winrt::hstring _toolTip;

		static WinrtXaml::DependencyProperty _commandProperty;
		static WinrtXaml::DependencyProperty _commandParameterProperty;
		static WinrtXaml::DependencyProperty _toolTipProperty;

		void OnLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args);
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ExtendedNavigationViewItem : ExtendedNavigationViewItemT<ExtendedNavigationViewItem, implementation::ExtendedNavigationViewItem>
	{
	};
}
