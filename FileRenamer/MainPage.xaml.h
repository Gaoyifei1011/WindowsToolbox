#pragma once

#include "pch.h"
#include "MainPage.g.h"
#include "Extensions/Command/RelayCommand.h"

#include <winrt/base.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Input.h>
#include <winrt/Windows.Foundation.Collections.h>

namespace winrt::FileRenamer::implementation
{
	struct MainPage : MainPageT<MainPage>
	{
		MainPage();

		int32_t MyProperty();
		void MyProperty(int32_t value);
		ICommand ClickCommand();
		void HyperlinkButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);

	private:
		HICON LoadLocalExeIcon(LPCWSTR exeFile);
		ICommand _clickCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainPage : MainPageT<MainPage, implementation::MainPage>
	{
	};
}
