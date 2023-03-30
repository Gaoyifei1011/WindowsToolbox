#pragma once

#include "pch.h"
#include "SettingsPage.h"
#include "SettingsPage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	SettingsPage::SettingsPage()
	{
		InitializeComponent();
	};
}
