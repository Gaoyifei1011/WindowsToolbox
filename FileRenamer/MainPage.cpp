#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	MainPage::MainPage()
	{
		InitializeComponent();

		_viewModel = make<winrt::FileRenamer::implementation::MainViewModel>();

		_fileName = AppResourceService.GetLocalized(L"Window/FileName");
		_extensionName = AppResourceService.GetLocalized(L"Window/ExtensionName");
		_upperAndLowerCase = AppResourceService.GetLocalized(L"Window/UpperAndLowerCase");
		_fileProperties = AppResourceService.GetLocalized(L"Window/FileProperties");
		_about = AppResourceService.GetLocalized(L"Window/About");
		_settings = AppResourceService.GetLocalized(L"Window/Settings");

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