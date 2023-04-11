#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	MainPage::MainPage()
	{
		InitializeComponent();

		_viewModel = make<winrt::FileRenamer::implementation::MainViewModel>();

		_fileName = AppResourcesService.GetLocalized(L"Window/FileName");
		_extensionName = AppResourcesService.GetLocalized(L"Window/ExtensionName");
		_upperAndLowerCase = AppResourcesService.GetLocalized(L"Window/UpperAndLowerCase");
		_fileProperties = AppResourcesService.GetLocalized(L"Window/FileProperties");
		_about = AppResourcesService.GetLocalized(L"Window/About");
		_settings = AppResourcesService.GetLocalized(L"Window/Settings");

		AppNavigationService.NavigationFrame(this->MainFrame());
	}

	winrt::FileRenamer::MainViewModel MainPage::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring MainPage::FileName()
	{
		return _fileName;
	}

	winrt::hstring MainPage::ExtensionName()
	{
		return _extensionName;
	}

	winrt::hstring MainPage::UpperAndLowerCase()
	{
		return _upperAndLowerCase;
	}

	winrt::hstring MainPage::FileProperties()
	{
		return _fileProperties;
	}

	winrt::hstring MainPage::About()
	{
		return _about;
	}

	winrt::hstring MainPage::Settings()
	{
		return _settings;
	}
}