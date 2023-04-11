#pragma once

#include <winrt/base.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "Models/About/ReferenceKeyValuePairModel.h"
#include "ReferenceViewModel.g.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
}

namespace winrt::FileRenamer::implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel>
	{
	public:
		ReferenceViewModel();

		winrt::WinrtCollections::IVector<winrt::FileRenamer::ReferenceKeyValuePairModel> ReferenceDict();

	private:
		winrt::WinrtCollections::IVector<winrt::FileRenamer::ReferenceKeyValuePairModel> _referenceDict{ winrt::single_threaded_vector< winrt::FileRenamer::ReferenceKeyValuePairModel >() };
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel, implementation::ReferenceViewModel>
	{
	};
}
