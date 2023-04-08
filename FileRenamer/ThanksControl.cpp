#include "pch.h"
#include "ThanksControl.h"
#include "ThanksControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	ThanksControl::ThanksControl()
	{
		InitializeComponent();

		_thanks = AppResourcesService.GetLocalized(L"About/Thanks");
	}

	hstring ThanksControl::Thanks()
	{
		return _thanks;
	}
}