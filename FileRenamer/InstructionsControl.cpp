#include "pch.h"
#include "InstructionsControl.h"
#include "InstructionsControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	InstructionsControl::InstructionsControl()
	{
		InitializeComponent();

		_useInstruction = AppResourcesService.GetLocalized(L"About/UseInstruction");
	}

	hstring InstructionsControl::UseInstruction()
	{
		return _useInstruction;
	}
}
