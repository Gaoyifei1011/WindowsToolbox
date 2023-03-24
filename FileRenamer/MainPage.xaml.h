#pragma once

#include "MainPage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct MainPage : MainPageT<MainPage>
	{
		MainPage();

		int32_t MyProperty();
		void MyProperty(int32_t value);
		void HyperlinkButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);

	private:
		HICON LoadLocalExeIcon(LPCWSTR exeFile);
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainPage : MainPageT<MainPage, implementation::MainPage>
	{
	};
}
