#pragma once

#include "winrt/base.h"
#include "FileNamePage.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage>
	{
	public:
		FileNamePage();

		hstring Title();

	private:
		hstring _title;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct FileNamePage : FileNamePageT<FileNamePage, implementation::FileNamePage>
	{
	};
}
