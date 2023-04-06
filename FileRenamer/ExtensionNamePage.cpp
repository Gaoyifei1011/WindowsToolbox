#pragma once

#include "pch.h"
#include "ExtensionNamePage.h"
#include "ExtensionNamePage.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	ExtensionNamePage::ExtensionNamePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"ExtensionName/Title");
	};

	hstring ExtensionNamePage::Title()
	{
		return _title;
	}
}