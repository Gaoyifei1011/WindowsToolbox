// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

#include "pch.h"
#include "MainWindow.xaml.h"
#if __has_include("MainWindow.g.cpp")
#include "MainWindow.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::FileRenamer::implementation
{
	MainWindow::MainWindow()
	{
		viewmodel = winrt::make<FileRenamer::implementation::MainWindowViewModel>();
		InitializeComponent();
	}

	FileRenamer::MainWindowViewModel MainWindow::ViewModel()
	{
		return viewmodel;
	}

	int32_t MainWindow::MyProperty()
	{
		throw hresult_not_implemented();
	}

	void MainWindow::MyProperty(int32_t /* value */)
	{
		throw hresult_not_implemented();
	}

	void MainWindow::myButton_Click(winrt::Windows::Foundation::IInspectable const& sender, RoutedEventArgs const& args)
	{
		myButton().Content(box_value(L"Clicked"));
	}

	void MainWindow::HyperlinkButton_Click(IInspectable const& sender, RoutedEventArgs const& args)
	{
		sender.as<winrt::Microsoft::UI::Xaml::Controls::HyperlinkButton>().Content(box_value(L"Clicked Over"));
	}
}