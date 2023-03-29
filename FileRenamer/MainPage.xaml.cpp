#pragma once

#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPage.g.cpp"
#include "WinMain.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	MainPage::MainPage()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::MainViewModel>();

		_fileName = AppResourcesService.GetLocalized(L"Window/FileName");
		_extensionName = AppResourcesService.GetLocalized(L"Window/ExtensionName");
		_upperAndLowerCase = AppResourcesService.GetLocalized(L"Window/UpperAndLowerCase");
		_fileProperties = AppResourcesService.GetLocalized(L"Window/FileProperties");
		_about = AppResourcesService.GetLocalized(L"Window/About");
	}

	FileRenamer::MainViewModel MainPage::ViewModel()
	{
		return _viewModel;
	}

	hstring MainPage::FileName()
	{
		return _fileName;
	}

	hstring MainPage::ExtensionName()
	{
		return _extensionName;
	}

	hstring MainPage::UpperAndLowerCase()
	{
		return _upperAndLowerCase;
	}

	hstring MainPage::FileProperties()
	{
		return _fileProperties;
	}

	hstring MainPage::About()
	{
		return _about;
	}
}