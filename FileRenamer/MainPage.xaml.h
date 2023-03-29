#pragma once

#include "pch.h"
#include "MainPage.g.h"
#include "Extensions/Command/RelayCommand.h"
#include "ViewModels/Window/MainViewModel.h"

#include <winrt/base.h>

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct MainPage : MainPageT<MainPage>
	{
		MainPage();
		FileRenamer::MainViewModel ViewModel();

		hstring FileName();
		hstring ExtensionName();
		hstring UpperAndLowerCase();
		hstring FileProperties();
		hstring About();

	private:
		FileRenamer::MainViewModel _viewModel;

		hstring _fileName;
		hstring _extensionName;
		hstring _upperAndLowerCase;
		hstring _fileProperties;
		hstring _about;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainPage : MainPageT<MainPage, implementation::MainPage>
	{
	};
}
