#include "pch.h"
#include "UpperAndLowerCasePage.h"
#include "UpperAndLowerCasePage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	UpperAndLowerCasePage::UpperAndLowerCasePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"UpperAndLowerCase/Title");
	};

	winrt::hstring UpperAndLowerCasePage::Title()
	{
		return _title;
	}
}