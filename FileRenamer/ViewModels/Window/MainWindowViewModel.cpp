#include "pch.h"
#include "MainWindowViewModel.h"
#if __has_include("MainWindowViewModel.g.cpp")
#include "MainWindowViewModel.g.cpp"
#endif

namespace winrt::FileRenamer::implementation
{
	MainWindowViewModel::MainWindowViewModel()
	{
		myProperty = 0;
	}

	int32_t MainWindowViewModel::MyProperty()
	{
		return myProperty;
	}

	void MainWindowViewModel::MyProperty(int32_t const& value)
	{
		if (myProperty != value)
		{
			myProperty = value;
		}
	}
}