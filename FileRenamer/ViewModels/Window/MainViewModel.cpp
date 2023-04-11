#include "pch.h"
#include "MainViewModel.h"
#include "MainViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	MainViewModel::MainViewModel()
	{
		_isBackEnabled = true;

		_navigationItemCommand = make<RelayCommand>([this](winrt::WinrtFoundation::IInspectable invokedItemTag)
			{
				if (invokedItemTag != nullptr)
				{
					for (uint32_t index = 0; index < AppNavigationService.NavigationItemList().Size(); index++)
					{
						if (AppNavigationService.NavigationItemList().GetAt(index).NavigationTag() == winrt::unbox_value<winrt::hstring>(invokedItemTag))
						{
							if (MainViewModel::SelectedItem() != AppNavigationService.NavigationItemList().GetAt(index).NavigationItem())
							{
								AppNavigationService.NavigateTo(AppNavigationService.NavigationItemList().GetAt(index).NavigationPage());
							}
						}
					}
				}
			});

		_pageDict.Insert(L"FileName", xaml_typename<FileRenamer::FileNamePage>());
		_pageDict.Insert(L"ExtensionName", xaml_typename<FileRenamer::ExtensionNamePage>());
		_pageDict.Insert(L"UpperAndLowerCase", xaml_typename<FileRenamer::UpperAndLowerCasePage>());
		_pageDict.Insert(L"FileProperties", xaml_typename<FileRenamer::FilePropertiesPage>());
		_pageDict.Insert(L"About", xaml_typename<FileRenamer::AboutPage>());
		_pageDict.Insert(L"Settings", xaml_typename<FileRenamer::SettingsPage>());
	};

	bool MainViewModel::IsBackEnabled()
	{
		return _isBackEnabled;
	}
	void MainViewModel::IsBackEnabled(bool const& value)
	{
		_isBackEnabled = value;
		m_propertyChanged(*this, winrt::WinrtData::PropertyChangedEventArgs{ L"IsBackEnabled" });
	}

	winrt::WinrtControls::NavigationViewItem MainViewModel::SelectedItem()
	{
		return _selectedItem;
	}
	void MainViewModel::SelectedItem(winrt::WinrtControls::NavigationViewItem const& value)
	{
		_selectedItem = value;
		m_propertyChanged(*this, winrt::WinrtData::PropertyChangedEventArgs{ L"SelectedItem" });
	}

	winrt::WinrtInput::ICommand MainViewModel::ClickCommand()
	{
		return _clickCommand;
	}

	winrt::WinrtInput::ICommand MainViewModel::NavigationItemCommand()
	{
		return _navigationItemCommand;
	}

	winrt::WinrtCollections::IMap<winrt::hstring, winrt::WinrtInterop::TypeName> MainViewModel::PageDict()
	{
		return _pageDict;
	}

	winrt::event_token MainViewModel::PropertyChanged(winrt::WinrtData::PropertyChangedEventHandler const& handler)
	{
		return m_propertyChanged.add(handler);
	}
	void MainViewModel::PropertyChanged(winrt::event_token const& token) noexcept
	{
		m_propertyChanged.remove(token);
	}

	/// <summary>
	/// 当后退按钮收到交互（如单击或点击）时发生
	/// </summary>
	void MainViewModel::OnNavigationViewBackRequested(winrt::WinrtControls::NavigationView const& sender, winrt::WinrtControls::NavigationViewBackRequestedEventArgs const& args)
	{
		AppNavigationService.NavigationFrom();
	}

	/// <summary>
	/// 导航控件加载完成后初始化内容
	/// </summary>
	void MainViewModel::OnNavigationViewLoaded(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtXaml::RoutedEventArgs const& args)
	{
		winrt::WinrtControls::NavigationView navigationView = sender.try_as<winrt::WinrtControls::NavigationView>();
		if (navigationView == nullptr)
		{
			return;
		}

		for (uint32_t index = 0; index < navigationView.MenuItems().Size(); index++)
		{
			winrt::WinrtControls::NavigationViewItem NavigationViewItem = navigationView.MenuItems().GetAt(index).try_as<winrt::WinrtControls::NavigationViewItem>();
			if (NavigationViewItem != nullptr)
			{
				winrt::hstring Tag = winrt::unbox_value<winrt::hstring>(NavigationViewItem.Tag());

				winrt::FileRenamer::NavigationModel item = make<winrt::FileRenamer::implementation::NavigationModel>();
				item.NavigationTag(Tag);
				item.NavigationItem(NavigationViewItem);
				item.NavigationPage(MainViewModel::PageDict().Lookup(Tag));
				AppNavigationService.NavigationItemList().Append(item);
			}
		}

		SelectedItem(AppNavigationService.NavigationItemList().GetAt(0).NavigationItem());
		AppNavigationService.NavigateTo(xaml_typename<FileNamePage>());
		IsBackEnabled(AppNavigationService.CanGoBack());
	}

	/// <summary>
	/// 导航完成后发生
	/// </summary>
	void MainViewModel::OnFrameNavigated(winrt::WinrtFoundation::IInspectable const& sender, winrt::WinrtNavigation::NavigationEventArgs const& args)
	{
		winrt::WinrtInterop::TypeName CurrentPageType = AppNavigationService.GetCurrentPageType();
		for (uint32_t index = 0; index < AppNavigationService.NavigationItemList().Size(); index++)
		{
			if (AppNavigationService.NavigationItemList().GetAt(index).NavigationPage().Name == CurrentPageType.Name)
			{
				MainViewModel::SelectedItem(AppNavigationService.NavigationItemList().GetAt(index).NavigationItem());
			}
		}
		IsBackEnabled(AppNavigationService.CanGoBack());
	}

	/// <summary>
	/// 导航失败时发生
	/// </summary>
	void MainViewModel::OnFrameNavgationFailed(IInspectable const& sender, winrt::WinrtNavigation::NavigationFailedEventArgs const& args)
	{
		//string str = to_string(AppResourcesService.GetLocalized(L"Window/NavigationFailed"));
		//throw AppStringFormatHelper.format(str, to_string(args.SourcePageType().Name));
	}
}