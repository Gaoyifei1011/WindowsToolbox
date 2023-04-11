#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "IntroductionControl.g.h"

namespace winrt::FileRenamer::implementation
{
	struct IntroductionControl : IntroductionControlT<IntroductionControl>
	{
	public:
		IntroductionControl();

		winrt::hstring BriefIntroduction();

	private:
		winrt::hstring _briefIntroduction;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct IntroductionControl : IntroductionControlT<IntroductionControl, implementation::IntroductionControl>
	{
	};
}
