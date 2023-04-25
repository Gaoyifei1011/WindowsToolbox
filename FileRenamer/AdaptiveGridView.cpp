#include "pch.h"
#include "AdaptiveGridView.h"
#include "AdaptiveGridView.g.cpp"

namespace winrt::FileRenamer::implementation
{
	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_desiredWidthProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"DesiredWidth",
		winrt::xaml_typename<double>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{
			winrt::box_value(0.0),
			winrt::WinrtXaml::PropertyChangedCallback{ &AdaptiveGridView::DesiredWidthChanged }
		}
	);

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_itemClickCommandProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"ItemClickCommand",
		winrt::xaml_typename<winrt::WinrtInput::ICommand>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{ nullptr,nullptr }
	);

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_itemHeightProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"ItemHeight",
		winrt::xaml_typename<double>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{ winrt::box_value(0.0), nullptr }
	);

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_itemWidthProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"ItemWidth",
		winrt::xaml_typename<double>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{ winrt::box_value(0.0) , nullptr }
	);

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_oneRowModeEnabledProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"OneRowModeEnabled",
		winrt::xaml_typename<bool>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{
			winrt::box_value(false),
			winrt::WinrtXaml::PropertyChangedCallback{ &AdaptiveGridView::OnOneRowModeEnabledChanged }
		}
	);

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::_stretchContentForSingleRowProperty = winrt::WinrtXaml::DependencyProperty::Register(
		L"StretchContentForSingleRow",
		winrt::xaml_typename<bool>(),
		winrt::xaml_typename<winrt::FileRenamer::AdaptiveGridView>(),
		winrt::WinrtXaml::PropertyMetadata{
			winrt::box_value(true),
			winrt::WinrtXaml::PropertyChangedCallback{ &AdaptiveGridView::OnStretchContentForSingleRowPropertyChanged }
		}
	);

	AdaptiveGridView::AdaptiveGridView()
	{
		InitializeComponent();

		_isLoaded = false;
		_savedOrientation = winrt::WinrtControls::Orientation::Horizontal;
		_savedVerticalScrollMode = winrt::WinrtControls::ScrollMode::Auto;
		_savedHorizontalScrollMode = winrt::WinrtControls::ScrollMode::Auto;
		_savedVerticalScrollBarVisibility = winrt::WinrtControls::ScrollBarVisibility::Auto;
		_savedHorizontalScrollBarVisibility = winrt::WinrtControls::ScrollBarVisibility::Auto;
		_needContainerMarginForLayout = false;
		_needToRestoreScrollStates = false;

		IsTabStop(false);
		this->SizeChanged({ this,&AdaptiveGridView::OnSizeChanged });
		this->ItemClick({ this,&AdaptiveGridView::OnItemClick });
		this->Items().VectorChanged({ this,&AdaptiveGridView::ItemsOnVectorChanged });
		this->Loaded({ this,&AdaptiveGridView::OnLoaded });
		this->Unloaded({ this,&AdaptiveGridView::OnUnloaded });
		UseLayoutRounding(false);
	}

	double AdaptiveGridView::DesiredWidth()
	{
		return winrt::unbox_value<double>(GetValue(_desiredWidthProperty));
	}
	void AdaptiveGridView::DesiredWidth(double const& value)
	{
		SetValue(_desiredWidthProperty, winrt::box_value(value));
	}

	winrt::WinrtInput::ICommand AdaptiveGridView::ItemClickCommand()
	{
		return winrt::unbox_value<winrt::WinrtInput::ICommand>(GetValue(_itemClickCommandProperty));
	}
	void AdaptiveGridView::ItemClickCommand(winrt::WinrtInput::ICommand const& value)
	{
		SetValue(_itemClickCommandProperty, value);
	}

	double AdaptiveGridView::ItemHeight()
	{
		return winrt::unbox_value<double>(GetValue(_itemHeightProperty));
	}
	void AdaptiveGridView::ItemHeight(double const& value)
	{
		SetValue(_itemHeightProperty, winrt::box_value(value));
	}

	double AdaptiveGridView::ItemWidth()
	{
		return winrt::unbox_value<double>(GetValue(_itemWidthProperty));
	}
	void AdaptiveGridView::ItemWidth(double const& value)
	{
		SetValue(_itemWidthProperty, winrt::box_value(value));
	}

	bool AdaptiveGridView::OneRowModeEnabled()
	{
		return winrt::unbox_value<bool>(GetValue(_oneRowModeEnabledProperty));
	}
	void AdaptiveGridView::OneRowModeEnabled(bool const& value)
	{
		SetValue(_oneRowModeEnabledProperty, winrt::box_value(value));
	}

	bool AdaptiveGridView::StretchContentForSingleRow()
	{
		return winrt::unbox_value<bool>(GetValue(_stretchContentForSingleRowProperty));
	}
	void AdaptiveGridView::StretchContentForSingleRow(bool const& value)
	{
		SetValue(_stretchContentForSingleRowProperty, winrt::box_value(value));
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::DesiredWidthProperty()
	{
		return _desiredWidthProperty;
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::ItemClickCommandProperty()
	{
		return _itemClickCommandProperty;
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::ItemHeightProperty()
	{
		return _itemHeightProperty;
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::ItemWidthProperty()
	{
		return _itemWidthProperty;
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::OneRowModeEnabledProperty()
	{
		return _oneRowModeEnabledProperty;
	}

	winrt::WinrtXaml::DependencyProperty AdaptiveGridView::StretchContentForSingleRowProperty()
	{
		return _stretchContentForSingleRowProperty;
	}

	/// <summary>准备指定的元素以显示指定的项。</summary>
	/// <param name="obj">用于显示指定项的元素。</param>
	/// <param name="item">要显示的项。</param>
	void AdaptiveGridView::PrepareContainerForItemOverride(winrt::WinrtXaml::DependencyObject obj, winrt::WinrtFoundation::IInspectable item)
	{
		__super::PrepareContainerForItemOverride(obj, item);

		winrt::WinrtXaml::FrameworkElement frameworkElement = obj.try_as<winrt::WinrtXaml::FrameworkElement>();
		if (frameworkElement != nullptr)
		{
			winrt::WinrtData::Binding binding;
			binding.Source(*this);
			winrt::WinrtXaml::PropertyPath propertyPath1(L"ItemHeight");
			binding.Path(propertyPath1);
			binding.Mode(winrt::WinrtData::BindingMode::TwoWay);
			winrt::WinrtData::Binding binding2;
			binding.Source(*this);
			winrt::WinrtXaml::PropertyPath propertyPath2(L"ItemWidth");
			binding.Path(propertyPath2);
			binding.Mode(winrt::WinrtData::BindingMode::TwoWay);
			frameworkElement.SetBinding(winrt::WinrtXaml::FrameworkElement::HeightProperty(), binding);
			frameworkElement.SetBinding(winrt::WinrtXaml::FrameworkElement::WidthProperty(), binding2);
		}

		winrt::WinrtControls::ContentControl contentControl = obj.try_as<winrt::WinrtControls::ContentControl>();
		if (contentControl != nullptr)
		{
			contentControl.HorizontalContentAlignment(winrt::WinrtXaml::HorizontalAlignment::Stretch);
			contentControl.VerticalContentAlignment(winrt::WinrtXaml::VerticalAlignment::Stretch);
		}

		if (_needContainerMarginForLayout)
		{
			_needContainerMarginForLayout = false;
			AdaptiveGridView::RecalculateLayout(AdaptiveGridView::ActualWidth());
		}
	}

	/// <summary>计算网格项的宽度。</summary>
	/// <param name="containerWidth">容器控件的宽度。</param>
	/// <returns>计算的项目宽度。</returns>
	double AdaptiveGridView::CalculateItemWidth(double containerWidth)
	{
		if (isnan(AdaptiveGridView::DesiredWidth()))
		{
			return AdaptiveGridView::DesiredWidth();
		}

		uint32_t num = CalculateColumns(containerWidth, AdaptiveGridView::DesiredWidth());
		if (this->Items() != nullptr && this->Items().Size() > 0 && this->Items().Size() < num && AdaptiveGridView::StretchContentForSingleRow())
		{
			num = this->Items().Size();
		}

		winrt::WinrtXaml::Thickness thickness = winrt::WinrtXaml::Thickness();

		winrt::WinrtXaml::Thickness itemMargin = AdaptiveHeightValueConverter::GetItemMargin(*this, thickness);
		if (itemMargin == thickness)
		{
			_needContainerMarginForLayout = true;
		}

		return (containerWidth / num) - itemMargin.Left - itemMargin.Right;
	}

	/// <summary>
	/// 每当应用程序代码或内部进程（如重建布局传递）调用 ApplyTemplate 时调用。简单来说，这意味着在应用中显示 UI 元素之前调用该方法。重写此方法以影响类的默认后模板逻辑。
	/// </summary>
	void AdaptiveGridView::OnApplyTemplate()
	{
		__super::OnApplyTemplate();
		AdaptiveGridView::OnOneRowModeEnabledChanged(*this, nullptr);
	}

	void AdaptiveGridView::ItemsOnVectorChanged(winrt::WinrtCollections::IObservableVector<winrt::WinrtFoundation::IInspectable> const& sender, winrt::WinrtCollections::IVectorChangedEventArgs const& args)
	{
		if (!isnan(AdaptiveGridView::ActualWidth()))
		{
			AdaptiveGridView::RecalculateLayout(AdaptiveGridView::ActualWidth());
		}
	}

	void AdaptiveGridView::OnItemClick(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtControls::ItemClickEventArgs const& args)
	{
		winrt::WinrtInput::ICommand itemClickCommand = AdaptiveGridView::ItemClickCommand();
		if (itemClickCommand != nullptr && itemClickCommand.CanExecute(args.ClickedItem()))
		{
			itemClickCommand.Execute(args.ClickedItem());
		}
	}

	void AdaptiveGridView::OnSizeChanged(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::SizeChangedEventArgs const& args)
	{
		if (AdaptiveGridView::HorizontalAlignment() == winrt::WinrtXaml::HorizontalAlignment::Stretch)
		{
			int num = AdaptiveGridView::CalculateColumns(args.PreviousSize().Width, AdaptiveGridView::DesiredWidth());
			int num2 = AdaptiveGridView::CalculateColumns(args.NewSize().Width, AdaptiveGridView::DesiredWidth());
			if (num != num2)
			{
				AdaptiveGridView::RecalculateLayout(args.NewSize().Width);
			}
		}
		else if (args.PreviousSize().Width != args.NewSize().Width)
		{
			AdaptiveGridView::RecalculateLayout(args.NewSize().Width);
		}
	}

	void AdaptiveGridView::OnLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args)
	{
		_isLoaded = true;
		AdaptiveGridView::DetermineOneRowMode();
	}

	void AdaptiveGridView::OnUnloaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args)
	{
		_isLoaded = false;
	}

	void AdaptiveGridView::DetermineOneRowMode()
	{
		if (!_isLoaded)
		{
			return;
		}

		winrt::WinrtControls::ItemsWrapGrid itemsWrapGrid = AdaptiveGridView::ItemsPanelRoot().as<winrt::WinrtControls::ItemsWrapGrid>();
		if (AdaptiveGridView::OneRowModeEnabled())
		{
			winrt::WinrtData::Binding binding;
			binding.Source(*this);
			binding.Path(winrt::WinrtXaml::PropertyPath(L"ItemHeight"));
			binding.Converter(winrt::FileRenamer::AdaptiveHeightValueConverter());
			binding.ConverterParameter(*this);

			if (itemsWrapGrid != nullptr)
			{
				_savedOrientation = itemsWrapGrid.Orientation();
				itemsWrapGrid.Orientation(winrt::WinrtControls::Orientation::Vertical);
			}

			SetBinding(winrt::WinrtXaml::FrameworkElement::MaxHeightProperty(), binding);
			_savedHorizontalScrollMode = winrt::WinrtControls::ScrollViewer::GetHorizontalScrollMode(*this);
			_savedVerticalScrollMode = winrt::WinrtControls::ScrollViewer::GetVerticalScrollMode(*this);
			_savedHorizontalScrollBarVisibility = winrt::WinrtControls::ScrollViewer::GetHorizontalScrollBarVisibility(*this);
			_savedVerticalScrollBarVisibility = winrt::WinrtControls::ScrollViewer::GetVerticalScrollBarVisibility(*this);
			_needToRestoreScrollStates = true;
			winrt::WinrtControls::ScrollViewer::SetVerticalScrollMode(*this, winrt::WinrtControls::ScrollMode::Disabled);
			winrt::WinrtControls::ScrollViewer::SetVerticalScrollBarVisibility(*this, winrt::WinrtControls::ScrollBarVisibility::Hidden);
			winrt::WinrtControls::ScrollViewer::SetHorizontalScrollBarVisibility(*this, winrt::WinrtControls::ScrollBarVisibility::Visible);
			winrt::WinrtControls::ScrollViewer::SetHorizontalScrollMode(*this, winrt::WinrtControls::ScrollMode::Disabled);
			return;
		}

		ClearValue(winrt::WinrtXaml::FrameworkElement::MaxHeightProperty());
		if (_needToRestoreScrollStates)
		{
			_needToRestoreScrollStates = false;
			if (itemsWrapGrid != nullptr)
			{
				itemsWrapGrid.Orientation(_savedOrientation);
				winrt::WinrtControls::ScrollViewer::SetVerticalScrollMode(*this, _savedVerticalScrollMode);
				winrt::WinrtControls::ScrollViewer::SetVerticalScrollBarVisibility(*this, _savedVerticalScrollBarVisibility);
				winrt::WinrtControls::ScrollViewer::SetHorizontalScrollBarVisibility(*this, _savedHorizontalScrollBarVisibility);
				winrt::WinrtControls::ScrollViewer::SetHorizontalScrollMode(*this, _savedHorizontalScrollMode);
			}
		}
	}

	void AdaptiveGridView::RecalculateLayout(double containerWidth)
	{
		winrt::WinrtControls::Panel itemsPanelRoot = ItemsPanelRoot();
		double num = itemsPanelRoot != nullptr ? itemsPanelRoot.Margin().Left + itemsPanelRoot.Margin().Right : 0.0;
		double num2 = AdaptiveGridView::Padding().Left + AdaptiveGridView::Padding().Right;
		double num3 = AdaptiveGridView::BorderThickness().Left + AdaptiveGridView::BorderThickness().Right;
		containerWidth = containerWidth - num2 - num - num3;
		if (containerWidth > 0.0)
		{
			double d = CalculateItemWidth(containerWidth);
			AdaptiveGridView::ItemWidth(floor(d));
		}
	}

	void AdaptiveGridView::OnOneRowModeEnabledChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args)
	{
		FileRenamer::AdaptiveGridView obj = d.as<FileRenamer::AdaptiveGridView>();
		winrt::FileRenamer::implementation::AdaptiveGridView* self = winrt::get_self<winrt::FileRenamer::implementation::AdaptiveGridView>(obj);
		self->DetermineOneRowMode();
	}

	void AdaptiveGridView::DesiredWidthChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args)
	{
		FileRenamer::AdaptiveGridView obj = d.as<FileRenamer::AdaptiveGridView>();
		winrt::FileRenamer::implementation::AdaptiveGridView* self = winrt::get_self<winrt::FileRenamer::implementation::AdaptiveGridView>(obj);
		self->RecalculateLayout(obj.ActualWidth());
	}

	void AdaptiveGridView::OnStretchContentForSingleRowPropertyChanged(winrt::WinrtXaml::DependencyObject const& d, winrt::WinrtXaml::DependencyPropertyChangedEventArgs const& args)
	{
		FileRenamer::AdaptiveGridView obj = d.as<FileRenamer::AdaptiveGridView>();
		winrt::FileRenamer::implementation::AdaptiveGridView* self = winrt::get_self<winrt::FileRenamer::implementation::AdaptiveGridView>(obj);
		self->RecalculateLayout(obj.ActualWidth());
	}

	int AdaptiveGridView::CalculateColumns(double containerWidth, double itemWidth)
	{
		int num = static_cast<int>(round(containerWidth / itemWidth));
		if (num == 0)
		{
			num = 1;
		}

		return num;
	}
}