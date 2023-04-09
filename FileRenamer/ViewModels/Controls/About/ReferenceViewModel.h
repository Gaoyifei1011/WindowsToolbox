#pragma once

#include "winrt/base.h"
#include "winrt/Windows.Foundation.Collections.h"
#include "Models/About/ReferenceKeyValuePairModel.h"
#include "ReferenceViewModel.g.h"

using namespace winrt;
using namespace winrt::Windows::Foundation;

namespace winrt::FileRenamer::implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel>
	{
	public:
		ReferenceViewModel();

		Collections::IVector<FileRenamer::ReferenceKeyValuePairModel> ReferenceDict();

	private:
		Collections::IVector<FileRenamer::ReferenceKeyValuePairModel> _referenceDict{ single_threaded_vector< FileRenamer::ReferenceKeyValuePairModel >() };
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceViewModel : ReferenceViewModelT<ReferenceViewModel, implementation::ReferenceViewModel>
	{
	};
}
