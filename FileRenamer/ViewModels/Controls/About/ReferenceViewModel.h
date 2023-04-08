#pragma once

#include "winrt/base.h"
#include "winrt/Windows.Foundation.Collections.h"
#include "ReferenceViewModel.g.h"

using namespace winrt;
using namespace winrt::Windows::Foundation;

namespace winrt::FileRenamer::implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel>
	{
	public:
		ReferenceViewModel();

		Collections::IMap<hstring, hstring> ReferenceDict();

	private:
		Collections::IMap<hstring, hstring> _referenceDict{ single_threaded_map<hstring,hstring>() };
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel, implementation::ReferenceViewModel>
	{
	};
}
