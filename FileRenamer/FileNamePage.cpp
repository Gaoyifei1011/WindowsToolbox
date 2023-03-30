#pragma once

#include "pch.h"
#include "FileNamePage.h"
#include "FileNamePage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	FileNamePage::FileNamePage()
	{
		InitializeComponent();
	};
}
