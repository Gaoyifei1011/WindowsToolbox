#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "PrecautionControl.g.h"

namespace winrt::FileRenamer::implementation
{
	struct PrecautionControl : PrecautionControlT<PrecautionControl>
	{
	public:
		PrecautionControl();

		winrt::hstring Precaution();

	private:
		winrt::hstring _precaution;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct PrecautionControl : PrecautionControlT<PrecautionControl, implementation::PrecautionControl>
	{
	};
}
