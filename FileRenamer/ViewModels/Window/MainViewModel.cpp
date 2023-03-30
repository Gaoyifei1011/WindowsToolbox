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
					hstring tag = unbox_value<hstring>(invokedItemTag);
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
		if (_isBackEnabled != value)
		{
			_isBackEnabled = value;
			m_propertyChanged(*this, PropertyChangedEventArgs{ L"IsBackEnabled" });
		}
	}

	IInspectable MainViewModel::SelectedItem()
	{
		return _selectedItem;
	}
	void MainViewModel::SelectedItem(IInspectable const& value)
	{
		if (_selectedItem != value)
		{
			_selectedItem = value;
			m_propertyChanged(*this, PropertyChangedEventArgs{ L"SelectedItem" });
		}
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
	}

	/// <summary>
	/// 导航控件加载完成后初始化内容
	/// </summary>
	void MainViewModel::OnNavigationViewLoaded(IInspectable const& sender, RoutedEventArgs const& args)
	{
	}

	/// <summary>
	/// 导航完成后发生
	/// </summary>
	void MainViewModel::OnFrameNavigated(IInspectable const& sender, NavigationEventArgs const& args)
	{
	}

	/// <summary>
	/// 导航失败时发生
	/// </summary>
	void MainViewModel::OnFrameNavgationFailed(IInspectable const& sender, NavigationFailedEventArgs const& args)
	{
		//throw AppStringFormatHelper.format(to_string(AppResourcesService.GetLocalized(L"Window/NavigationFailed")), to_string(args.SourcePageType().Name));
	}
}