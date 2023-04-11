#pragma once

#include <winrt/base.h>
#include <WinMain.h>

#include "InstructionsControl.g.h"

namespace winrt::FileRenamer::implementation
{
	struct InstructionsControl : InstructionsControlT<InstructionsControl>
	{
	public:
		InstructionsControl();

		winrt::hstring UseInstruction();

	private:
		winrt::hstring _useInstruction;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct InstructionsControl : InstructionsControlT<InstructionsControl, implementation::InstructionsControl>
	{
	};
}
