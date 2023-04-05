#pragma once

#include "UpperAndLowerCasePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage>
	{
		UpperAndLowerCasePage();
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct UpperAndLowerCasePage : UpperAndLowerCasePageT<UpperAndLowerCasePage, implementation::UpperAndLowerCasePage>
	{
	};
}
