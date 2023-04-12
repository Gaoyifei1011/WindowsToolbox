#include "pch.h"
#include "ThanksControl.h"
#include "ThanksControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThanksControl::ThanksControl()
	{
		InitializeComponent();

		_thanksTitle = AppResourcesService.GetLocalized(L"About/ThanksTitle");
	}

	winrt::hstring ThanksControl::ThanksTitle()
	{
		return _thanksTitle;
	}
}