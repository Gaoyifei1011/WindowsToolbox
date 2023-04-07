#include "pch.h"
#include "PrecautionControl.h"
#include "PrecautionControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	PrecautionControl::PrecautionControl()
	{
		InitializeComponent();

		_precaution = AppResourcesService.GetLocalized(L"About/Precaution");
	}

	hstring PrecautionControl::Precaution()
	{
		return _precaution;
	}
}
