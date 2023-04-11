#include "pch.h"
#include "ThanksControl.h"
#include "ThanksControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThanksControl::ThanksControl()
	{
		InitializeComponent();

		_thanks = AppResourcesService.GetLocalized(L"About/Thanks");
	}

	winrt::hstring ThanksControl::Thanks()
	{
		return _thanks;
	}
}