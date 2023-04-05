#pragma once

#include "FilePropertiesPage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage>
	{
		FilePropertiesPage();
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FilePropertiesPage : FilePropertiesPageT<FilePropertiesPage, implementation::FilePropertiesPage>
	{
	};
}
