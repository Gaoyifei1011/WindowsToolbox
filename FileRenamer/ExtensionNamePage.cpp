#include "pch.h"
#include "ExtensionNamePage.h"
#include "ExtensionNamePage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ExtensionNamePage::ExtensionNamePage()
	{
		InitializeComponent();

		_title = AppResourceService.GetLocalized(L"ExtensionName/Title");
	};

	winrt::hstring ExtensionNamePage::Title()
	{
		return _title;
	}
}