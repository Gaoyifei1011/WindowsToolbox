#pragma once

#include "pch.h"
#include "FileNamePage.h"
#include "FileNamePage.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	FileNamePage::FileNamePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"FileName/Title");
	};

	hstring FileNamePage::Title()
	{
		return _title;
	}
}