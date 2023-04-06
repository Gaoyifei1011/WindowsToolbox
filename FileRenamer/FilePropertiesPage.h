#pragma once

#include "winrt/base.h"
#include "FilePropertiesPage.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage>
	{
	public:
		FilePropertiesPage();

		hstring Title();

	private:
		hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage, implementation::FilePropertiesPage>
	{
	};
}
