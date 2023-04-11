#pragma once

#include <winrt/base.h>

#include "ReferenceKeyValuePairModel.g.h"

namespace winrt::FileRenamer::implementation
{
	struct ReferenceKeyValuePairModel : ReferenceKeyValuePairModelT<ReferenceKeyValuePairModel>
	{
	public:
		ReferenceKeyValuePairModel();
		ReferenceKeyValuePairModel(winrt::hstring const& key, winrt::hstring const& value);

		winrt::hstring Key();
		void Key(winrt::hstring const& value);

		hstring Value();
		void Value(winrt::hstring const& value);

	private:
		winrt::hstring _key;
		winrt::hstring _value;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ReferenceKeyValuePairModel : ReferenceKeyValuePairModelT<ReferenceKeyValuePairModel, implementation::ReferenceKeyValuePairModel>
	{
	};
}
