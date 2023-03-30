#pragma once

#include "pch.h"
#include "AboutPage.h"
#include "AboutPage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	AboutPage::AboutPage() 
	{
		InitializeComponent();
	};
}
