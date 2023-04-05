#pragma once

#include "ExtensionNamePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage>
	{
		ExtensionNamePage();
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ExtensionNamePage : ExtensionNamePageT<ExtensionNamePage, implementation::ExtensionNamePage>
	{
	};
}
