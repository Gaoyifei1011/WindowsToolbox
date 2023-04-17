#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.Xaml.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Data.h>
#include <winrt/Windows.UI.Xaml.Input.h>

#include "AdaptiveGridView.g.h"
#include "Converters/Controls/AdaptiveHeightValueConverter.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtData = Windows::UI::Xaml::Data;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtXaml = Windows::UI::Xaml;
}

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	/// 自适应窗口宽度的网格控件
	/// </summary>
	struct AdaptiveGridView : AdaptiveGridViewT<AdaptiveGridView>
	{
	public:
		AdaptiveGridView();

		/// <summary>
		/// 获取或设置每个项的所需宽度
		/// </summary>
		double DesiredWidth();
		void DesiredWidth(double const& value);

		/// <summary>
		/// 获取或设置在单击项且 IsItemClickEnabled 属性为 true 时要执行的命令
		/// </summary>
		winrt::WinrtInput::ICommand ItemClickCommand();
		void ItemClickCommand(winrt::WinrtInput::ICommand const& value);

		/// <summary>
		/// 获取或设置网格中每个项的高度
		/// </summary>
		double ItemHeight();
		void ItemHeight(double const& value);

		/// <summary>
		/// 获取或设置网格中每个项的宽度
		/// </summary>
		double ItemWidth();
		void ItemWidth(double const& value);

		/// <summary>
		/// 获取或设置一个值，该值指示是否只应显示一行
		/// </summary>
		bool OneRowModeEnabled();
		void OneRowModeEnabled(bool const& value);

		bool StretchContentForSingleRow();
		void StretchContentForSingleRow(bool const& value);

		void DetermineOneRowMode();
		void PrepareContainerForItemOverride(winrt::WinrtXaml::DependencyObject obj, winrt::WinrtFoundation::IInspectable item);
		void OnApplyTemplate();

		static winrt::WinrtXaml::DependencyProperty DesiredWidthProperty();
		static winrt::WinrtXaml::DependencyProperty ItemClickCommandProperty();
		static winrt::WinrtXaml::DependencyProperty ItemHeightProperty();
		static winrt::WinrtXaml::DependencyProperty ItemWidthProperty();
		static winrt::WinrtXaml::DependencyProperty OneRowModeEnabledProperty();
		static winrt::WinrtXaml::DependencyProperty StretchContentForSingleRowProperty();

	private:
		bool _isLoaded;
		winrt::WinrtControls::ScrollMode _savedVerticalScrollMode;
		winrt::WinrtControls::ScrollMode _savedHorizontalScrollMode;
		winrt::WinrtControls::ScrollBarVisibility _savedVerticalScrollBarVisibility;
		winrt::WinrtControls::ScrollBarVisibility _savedHorizontalScrollBarVisibility;
		winrt::WinrtControls::Orientation _savedOrientation;
		bool _needToRestoreScrollStates;
		bool _needContainerMarginForLayout;

		static winrt::WinrtXaml::DependencyProperty _desiredWidthProperty;
		static winrt::WinrtXaml::DependencyProperty _itemClickCommandProperty;
		static winrt::WinrtXaml::DependencyProperty _itemHeightProperty;
		static winrt::WinrtXaml::DependencyProperty _itemWidthProperty;
		static winrt::WinrtXaml::DependencyProperty _oneRowModeEnabledProperty;
		static winrt::WinrtXaml::DependencyProperty _stretchContentForSingleRowProperty;

		double CalculateItemWidth(double containerWidth);
		void ItemsOnVectorChanged(winrt::WinrtCollections::IObservableVector<winrt::WinrtFoundation::IInspectable> const& sender, winrt::WinrtCollections::IVectorChangedEventArgs const& args);
		void OnItemClick(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtControls::ItemClickEventArgs const& args);
		void OnSizeChanged(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::SizeChangedEventArgs const& args);
		void OnLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args);
		void OnUnloaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args);

		void RecalculateLayout(double containerWidth);
		int CalculateColumns(double containerWidth, double itemWidth);

		static void OnOneRowModeEnabledChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args);
		static void DesiredWidthChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args);
		static void OnStretchContentForSingleRowPropertyChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args);
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AdaptiveGridView : AdaptiveGridViewT<AdaptiveGridView, implementation::AdaptiveGridView>
	{
	};
}
