#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "ReferenceControl.g.h"
#include "ViewModels/Controls/About/ReferenceViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct ReferenceControl : ReferenceControlT<ReferenceControl>
	{
	public:
		ReferenceControl();

		winrt::FileRenamer::ReferenceViewModel ViewModel();

		winrt::hstring Reference();

	private:
		winrt::FileRenamer::ReferenceViewModel _viewModel;

		winrt::hstring _reference;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceControl : ReferenceControlT<ReferenceControl, implementation::ReferenceControl>
	{
	};
}
