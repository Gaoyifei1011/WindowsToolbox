#pragma once

#include <winrt/base.h>

#include "FileNamePage.g.h"
#include "WinMain.h"

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
