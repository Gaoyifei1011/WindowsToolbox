#include "pch.h"
#include "IntroductionControl.h"
#include "IntroductionControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	IntroductionControl::IntroductionControl()
	{
		InitializeComponent();

		_briefIntroduction = AppResourcesService.GetLocalized(L"About/BriefIntroduction");
	}

	hstring IntroductionControl::BriefIntroduction()
	{
		return _briefIntroduction;
	}
}
