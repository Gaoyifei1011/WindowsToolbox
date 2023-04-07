#include "pch.h"
#include "ReferenceControl.h"
#include "ReferenceControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	ReferenceControl::ReferenceControl()
	{
		InitializeComponent();

		_reference = AppResourcesService.GetLocalized(L"About/Reference");
	}

	hstring ReferenceControl::Reference()
	{
		return _reference;
	}
}
