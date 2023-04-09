#pragma once

#include "pch.h"
#include "MainViewModel.h"
#include "MainViewModel.g.cpp"
#include "Helpers/Root/StringFormatHelper.h"

using namespace std;
using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::FileRenamer::implementation
{
	MainViewModel::MainViewModel()
	{
		_isBackEnabled = true;

		_navigationItemCommand = make<RelayCommand>([this](IInspectable invokedItemTag)
			{
				if (invokedItemTag != nullptr)
				{
					for (uint32_t index = 0; index < AppNavigationService.NavigationItemList().Size(); index++)
					{
						if (AppNavigationService.NavigationItemList().GetAt(index).NavigationTag() == unbox_value<hstring>(invokedItemTag))
						{
							if (MainViewModel::SelectedItem() != AppNavigationService.NavigationItemList().GetAt(index).NavigationItem())
							{
								AppNavigationService.NavigateTo(AppNavigationService.NavigationItemList().GetAt(index).NavigationPage());
							}
						}
					}
				}
			});

		_clickCommand = make<RelayCommand>([this](IInspectable parameter)
			{
				TCHAR szFullPath[MAX_PATH];
				ZeroMemory(szFullPath, MAX_PATH);
				GetModuleFileName(NULL, szFullPath, MAX_PATH);
				MessageBox(ApplicationRoot->MainWindow.Handle(), L"测试对话框", szFullPath, MB_OK);
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
		m_propertyChanged(*this, PropertyChangedEventArgs{ L"IsBackEnabled" });
	}

	NavigationViewItem MainViewModel::SelectedItem()
	{
		return _selectedItem;
	}
	void MainViewModel::SelectedItem(NavigationViewItem const& value)
	{
		_selectedItem = value;
		m_propertyChanged(*this, PropertyChangedEventArgs{ L"SelectedItem" });
	}

	ICommand MainViewModel::ClickCommand()
	{
		return _clickCommand;
	}

	ICommand MainViewModel::NavigationItemCommand()
	{
		return _navigationItemCommand;
	}

	Collections::IMap<hstring, TypeName> MainViewModel::PageDict()
	{
		return _pageDict;
	}

	event_token MainViewModel::PropertyChanged(PropertyChangedEventHandler const& handler)
	{
		return m_propertyChanged.add(handler);
	}
	void MainViewModel::PropertyChanged(event_token const& token) noexcept
	{
		m_propertyChanged.remove(token);
	}

	/// <summary>
	/// 当后退按钮收到交互（如单击或点击）时发生
	/// </summary>
	void MainViewModel::OnNavigationViewBackRequested(NavigationView const& sender, NavigationViewBackRequestedEventArgs const& args)
	{
		AppNavigationService.NavigationFrom();
	}

	/// <summary>
	/// 导航控件加载完成后初始化内容
	/// </summary>
	void MainViewModel::OnNavigationViewLoaded(IInspectable const& sender, RoutedEventArgs const& args)
	{
		NavigationView navigationView = sender.try_as<NavigationView>();
		if (navigationView == nullptr)
		{
			return;
		}

		for (uint32_t index = 0; index < navigationView.MenuItems().Size(); index++)
		{
			NavigationViewItem navigationViewItem = navigationView.MenuItems().GetAt(index).try_as<NavigationViewItem>();
			if (navigationViewItem != nullptr)
			{
				hstring Tag = unbox_value<hstring>(navigationViewItem.Tag());

				FileRenamer::NavigationModel item = make<FileRenamer::implementation::NavigationModel>();
				item.NavigationTag(Tag);
				item.NavigationItem(navigationViewItem);
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
	void MainViewModel::OnFrameNavigated(IInspectable const& sender, NavigationEventArgs const& args)
	{
		TypeName CurrentPageType = AppNavigationService.GetCurrentPageType();
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
	void MainViewModel::OnFrameNavgationFailed(IInspectable const& sender, NavigationFailedEventArgs const& args)
	{
		//string str = to_string(AppResourcesService.GetLocalized(L"Window/NavigationFailed"));
		//throw AppStringFormatHelper.format(str, to_string(args.SourcePageType().Name));
	}
}