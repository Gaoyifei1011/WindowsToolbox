#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "ThanksControl.g.h"
#include "ViewModels/Controls/About/ThanksViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl>
	{
	public:
		ThanksControl();

		winrt::FileRenamer::ThanksViewModel ViewModel();

		winrt::hstring ThanksTitle();

	private:
		winrt::FileRenamer::ThanksViewModel _viewModel;

		winrt::hstring _thanksTitle;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl, implementation::ThanksControl>
	{
	};
}
