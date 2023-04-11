#include "pch.h"
#include "IntroductionControl.h"
#include "IntroductionControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	IntroductionControl::IntroductionControl()
	{
		InitializeComponent();

		_briefIntroduction = AppResourcesService.GetLocalized(L"About/BriefIntroduction");
	}

	winrt::hstring IntroductionControl::BriefIntroduction()
	{
		return _briefIntroduction;
	}
}