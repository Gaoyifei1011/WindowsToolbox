#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "ExtensionNamePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage>
	{
	public:
		ExtensionNamePage();

		winrt::hstring Title();

	private:
		winrt::hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage, implementation::ExtensionNamePage>
	{
	};
}
