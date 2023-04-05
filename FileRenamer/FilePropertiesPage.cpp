#pragma once

#include "pch.h"
#include "FilePropertiesPage.h"
#include "FilePropertiesPage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	FilePropertiesPage::FilePropertiesPage()
	{
		InitializeComponent();
	};
}