#pragma once

#include <winrt/base.h>

#include "global.h"
#include "UpperAndLowerCasePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage>
	{
	public:
		UpperAndLowerCasePage();

		winrt::hstring Title();

	private:
		winrt::hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage, implementation::UpperAndLowerCasePage>
	{
	};
}
