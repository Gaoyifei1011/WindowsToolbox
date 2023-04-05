#pragma once

#include "pch.h"
#include "ExtensionNamePage.h"
#include "ExtensionNamePage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	ExtensionNamePage::ExtensionNamePage()
	{
		InitializeComponent();
	};
}