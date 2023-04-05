#pragma once

#include "pch.h"
#include "UpperAndLowerCasePage.h"
#include "UpperAndLowerCasePage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	UpperAndLowerCasePage::UpperAndLowerCasePage()
	{
		InitializeComponent();
	};
}