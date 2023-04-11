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

		winrt::hstring Thanks();

	private:
		winrt::hstring _thanks;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl, implementation::ThanksControl>
	{
	};
}
