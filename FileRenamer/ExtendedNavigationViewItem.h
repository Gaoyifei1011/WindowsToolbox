#pragma once

#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "winrt/Windows.UI.Xaml.Input.h"
#include "ExtendedNavigationViewItem.g.h"

using namespace winrt;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Input;

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	/// 扩展后的导航控件项，附带命令绑定
	/// </summary>
	struct ExtendedNavigationViewItem : ExtendedNavigationViewItemT<ExtendedNavigationViewItem>
	{
	public:
		ExtendedNavigationViewItem();

		ICommand Command();
		void Command(ICommand const& value);

		IInspectable CommandParameter();
		void CommandParameter(IInspectable const& value);

		hstring ToolTip();
		void ToolTip(hstring const& value);

		static DependencyProperty CommandProperty();
		static DependencyProperty CommandParameterProperty();
		static DependencyProperty ToolTipProperty();

	private:
		ICommand _command;
		IInspectable _commandParameter;
		hstring _toolTip;

		static DependencyProperty _commandProperty;
		static DependencyProperty _commandParameterProperty;
		static DependencyProperty _toolTipProperty;

		void OnLoaded(IInspectable const& sender, RoutedEventArgs const& args);
		void OnItemTapped(IInspectable const& sender, TappedRoutedEventArgs const& args);
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ExtendedNavigationViewItem : ExtendedNavigationViewItemT<ExtendedNavigationViewItem, implementation::ExtendedNavigationViewItem>
	{
	};
}
