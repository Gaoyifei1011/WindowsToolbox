#pragma once

#include "winrt/base.h"
#include "AboutPage.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct AboutPage : AboutPageT<AboutPage>
	{
	public:
		AboutPage();

		hstring Title();

	private:
		hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AboutPage : AboutPageT<AboutPage, implementation::AboutPage>
	{
	};
}
