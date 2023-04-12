#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "ThanksControl.g.h"

namespace winrt::FileRenamer::implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl>
	{
	public:
		ThanksControl();

		winrt::hstring ThanksTitle();

	private:
		winrt::hstring _thanksTitle;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl, implementation::ThanksControl>
	{
	};
}
