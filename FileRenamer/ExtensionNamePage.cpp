#include "pch.h"
#include "ExtensionNamePage.h"
#include "ExtensionNamePage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ExtensionNamePage::ExtensionNamePage()
	{
		InitializeComponent();

		_title = AppResourcesService.GetLocalized(L"ExtensionName/Title");
	};

	winrt::hstring ExtensionNamePage::Title()
	{
		return _title;
	}
}