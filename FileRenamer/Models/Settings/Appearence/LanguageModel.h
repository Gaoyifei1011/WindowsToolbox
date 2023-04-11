#pragma once

#include "LanguageModel.g.h"

namespace winrt::FileRenamer::implementation
{
	struct LanguageModel : LanguageModelT<LanguageModel>
	{
		LanguageModel();

	public:
		winrt::hstring DisplayName();
		void DisplayName(winrt::hstring const& value);

		winrt::hstring InternalName();
		void InternalName(winrt::hstring const& value);

	private:
		winrt::hstring _displayName;
		winrt::hstring _internalName;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct LanguageModel : LanguageModelT<LanguageModel, implementation::LanguageModel>
	{
	};
}
