#pragma once

#include "winrt/base.h"
#include "ExtensionNamePage.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage>
	{
	public:
		ExtensionNamePage();

		hstring Title();

	private:
		hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage, implementation::ExtensionNamePage>
	{
	};
}
