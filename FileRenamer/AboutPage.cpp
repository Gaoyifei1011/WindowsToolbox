#pragma once

#include "pch.h"
#include "AboutPage.h"
#include "AboutPage.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	AboutPage::AboutPage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"About/Title");
	};

	hstring AboutPage::Title()
	{
		return _title;
	}
}