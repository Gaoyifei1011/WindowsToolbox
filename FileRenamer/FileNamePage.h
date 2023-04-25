#pragma once

#include <winrt/base.h>

#include "global.h"
#include "FileNamePage.g.h"

namespace winrt::FileRenamer::implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage>
	{
	public:
		FileNamePage();

		winrt::hstring Title();

	private:
		winrt::hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage, implementation::FileNamePage>
	{
	};
}
