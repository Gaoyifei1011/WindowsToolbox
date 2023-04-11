﻿#pragma once

#include "pch.h"
#include "FilePropertiesPage.h"
#include "FilePropertiesPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	FilePropertiesPage::FilePropertiesPage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"FileProperties/Title");
	};

	hstring FilePropertiesPage::Title()
	{
		return _title;
	}
}