#include "pch.h"
#include "ThanksControl.h"
#include "ThanksControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThanksControl::ThanksControl()
	{
		InitializeComponent();

		_viewModel = winrt::make<FileRenamer::implementation::ThanksViewModel>();

		_thanksTitle = AppResourcesService.GetLocalized(L"About/ThanksTitle");
	}

	winrt::FileRenamer::ThanksViewModel ThanksControl::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring ThanksControl::ThanksTitle()
	{
		return _thanksTitle;
	}
}