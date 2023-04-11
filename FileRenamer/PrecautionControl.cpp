#include "pch.h"
#include "PrecautionControl.h"
#include "PrecautionControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	PrecautionControl::PrecautionControl()
	{
		InitializeComponent();

		_precaution = AppResourcesService.GetLocalized(L"About/Precaution");
	}

	winrt::hstring PrecautionControl::Precaution()
	{
		return _precaution;
	}
}