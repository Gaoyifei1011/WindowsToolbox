#pragma once

#include "FileNamePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage>
	{
		FileNamePage();
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage, implementation::FileNamePage>
	{
	};
}
