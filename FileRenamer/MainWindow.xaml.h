// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

#pragma once
#include "MainWindow.g.h"
#include "ViewModels/Window/MainWindowViewModel.h"
#include "MainWindowViewModel.g.h"

namespace winrt::FileRenamer::implementation
{
	struct MainWindow : MainWindowT<MainWindow>
	{
		MainWindow();

		FileRenamer::MainWindowViewModel ViewModel();

		int32_t MyProperty();
		void MyProperty(int32_t value);

		void myButton_Click(Windows::Foundation::IInspectable const& sender, Microsoft::UI::Xaml::RoutedEventArgs const& args);
		void HyperlinkButton_Click(Windows::Foundation::IInspectable const& sender, Microsoft::UI::Xaml::RoutedEventArgs const& e);

	private:
		FileRenamer::MainWindowViewModel viewmodel{ nullptr };
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct MainWindow : MainWindowT<MainWindow, implementation::MainWindow>
	{
	};
}
