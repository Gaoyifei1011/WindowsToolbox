#pragma once

#include "pch.h"
#include "UpperAndLowerCasePage.h"
#include "UpperAndLowerCasePage.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	UpperAndLowerCasePage::UpperAndLowerCasePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"UpperAndLowerCase/Title");
	};

	hstring UpperAndLowerCasePage::Title()
	{
		return _title;
	}
}