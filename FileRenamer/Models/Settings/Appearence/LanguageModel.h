#pragma once

#include <winrt/base.h>

#include "LanguageModel.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct LanguageModel : LanguageModelT<LanguageModel>
	{
		LanguageModel();

	public:
		hstring DisplayName();
		void DisplayName(hstring const& value);

		hstring InternalName();
		void InternalName(hstring const& value);

	private:
		hstring _displayName;
		hstring _internalName;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct LanguageModel : LanguageModelT<LanguageModel, implementation::LanguageModel>
	{
	};
}
