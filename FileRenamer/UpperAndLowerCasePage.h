#pragma once

#include "winrt/base.h"
#include "UpperAndLowerCasePage.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage>
	{
	public:
		UpperAndLowerCasePage();

		hstring Title();

	private:
		hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage, implementation::UpperAndLowerCasePage>
	{
	};
}
