#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "ReferenceControl.g.h"
#include "ViewModels/Controls/About/ReferenceViewModel.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct ReferenceControl : ReferenceControlT<ReferenceControl>
	{
	public:
		ReferenceControl();

		FileRenamer::ReferenceViewModel ViewModel();

		hstring Reference();

	private:
		FileRenamer::ReferenceViewModel _viewModel;

		hstring _reference;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceControl : ReferenceControlT<ReferenceControl, implementation::ReferenceControl>
	{
	};
}
