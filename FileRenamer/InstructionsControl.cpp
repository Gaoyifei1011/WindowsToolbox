#include "pch.h"
#include "InstructionsControl.h"
#include "InstructionsControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	InstructionsControl::InstructionsControl()
	{
		InitializeComponent();

		_useInstruction = AppResourcesService.GetLocalized(L"About/UseInstruction");
	}

	winrt::hstring InstructionsControl::UseInstruction()
	{
		return _useInstruction;
	}
}