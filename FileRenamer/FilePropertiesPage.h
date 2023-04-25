#pragma once

#include <winrt/base.h>

#include "global.h"
#include "FilePropertiesPage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage>
	{
	public:
		FilePropertiesPage();

		winrt::hstring Title();

	private:
		winrt::hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage, implementation::FilePropertiesPage>
	{
	};
}
