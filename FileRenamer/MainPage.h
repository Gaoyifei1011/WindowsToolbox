#pragma once

#include <winrt/base.h>

#include "global.h"
#include "pch.h"
#include "MainPage.g.h"
#include "Extensions/Command/RelayCommand.h"
#include "ViewModels/Window/MainViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct MainPage : MainPageT<MainPage>
	{
		MainPage();
		winrt::FileRenamer::MainViewModel ViewModel();

		winrt::hstring FileName();
		winrt::hstring ExtensionName();
		winrt::hstring UpperAndLowerCase();
		winrt::hstring FileProperties();
		winrt::hstring About();
		winrt::hstring Settings();

	private:
		winrt::FileRenamer::MainViewModel _viewModel;

		winrt::hstring _fileName;
		winrt::hstring _extensionName;
		winrt::hstring _upperAndLowerCase;
		winrt::hstring _fileProperties;
		winrt::hstring _about;
		winrt::hstring _settings;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainPage : MainPageT<MainPage, implementation::MainPage>
	{
	};
}
