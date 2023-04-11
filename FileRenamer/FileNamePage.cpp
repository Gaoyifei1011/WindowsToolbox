#pragma once

#include "pch.h"
#include "FileNamePage.h"
#include "FileNamePage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	FileNamePage::FileNamePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"FileName/Title");
	};

	winrt::hstring FileNamePage::Title()
	{
		return _title;
	}
}