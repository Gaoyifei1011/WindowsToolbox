#pragma once

#include "pch.h"
#include "AboutPage.h"
#include "AboutPage.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	AboutPage::AboutPage()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::AboutViewModel>();

		_title = AppResourcesService.GetLocalized(L"About/Title");
		_briefIntroduction = AppResourcesService.GetLocalized(L"About/BriefIntroduction");
		_reference = AppResourcesService.GetLocalized(L"About/Reference");
		_useInstruction = AppResourcesService.GetLocalized(L"About/UseInstruction");
		_precaution = AppResourcesService.GetLocalized(L"About/Precaution");
		_settingsHelp = AppResourcesService.GetLocalized(L"About/SettingsHelp");
		_thanks = AppResourcesService.GetLocalized(L"About/Thanks");
		_quickOperation = AppResourcesService.GetLocalized(L"About/QuickOperation");
		_createDesktopShortcut = AppResourcesService.GetLocalized(L"About/CreateDesktopShortcut");
		_createDesktopShortcutToolTip = AppResourcesService.GetLocalized(L"About/CreateDesktopShortcutToolTip");
		_pinToStartScreen = AppResourcesService.GetLocalized(L"About/PinToStartScreen");
		_pinToStartScreenToolTip = AppResourcesService.GetLocalized(L"About/PinToStartScreenToolTip");
		_pinToTaskbar = AppResourcesService.GetLocalized(L"About/PinToTaskbar");
		_pinToTaskbarToolTip = AppResourcesService.GetLocalized(L"About/PinToTaskbarToolTip");
		_updateAndLicensing = AppResourcesService.GetLocalized(L"About/UpdateAndLicensing");
		_showReleaseNotes = AppResourcesService.GetLocalized(L"About/ShowReleaseNotes");
		_showReleaseNotesToolTip = AppResourcesService.GetLocalized(L"About/ShowReleaseNotesToolTip");
		_showLicense = AppResourcesService.GetLocalized(L"About/ShowLicense");
		_showLicenseToolTip = AppResourcesService.GetLocalized(L"About/ShowLicenseToolTip");
	};

	FileRenamer::AboutViewModel AboutPage::ViewModel()
	{
		return _viewModel;
	}

	hstring AboutPage::Title()
	{
		return _title;
	}

	hstring AboutPage::BriefIntroduction()
	{
		return _briefIntroduction;
	}

	hstring AboutPage::Reference()
	{
		return _reference;
	}

	hstring AboutPage::UseInstruction()
	{
		return _useInstruction;
	}

	hstring AboutPage::Precaution()
	{
		return _precaution;
	}

	hstring AboutPage::SettingsHelp()
	{
		return _settingsHelp;
	}

	hstring AboutPage::Thanks()
	{
		return _thanks;
	}

	hstring AboutPage::QuickOperation()
	{
		return _quickOperation;
	}

	hstring AboutPage::CreateDesktopShortcut()
	{
		return _createDesktopShortcut;
	}

	hstring AboutPage::CreateDesktopShortcutToolTip()
	{
		return _createDesktopShortcutToolTip;
	}

	hstring AboutPage::PinToStartScreen()
	{
		return _pinToStartScreen;
	}

	hstring AboutPage::PinToStartScreenToolTip()
	{
		return _pinToStartScreenToolTip;
	}

	hstring AboutPage::PinToTaskbar()
	{
		return _pinToTaskbar;
	}

	hstring AboutPage::PinToTaskbarToolTip()
	{
		return _pinToTaskbarToolTip;
	}

	hstring AboutPage::UpdateAndLicensing()
	{
		return _updateAndLicensing;
	}

	hstring AboutPage::ShowReleaseNotes()
	{
		return _showReleaseNotes;
	}

	hstring AboutPage::ShowReleaseNotesToolTip()
	{
		return _showReleaseNotesToolTip;
	}

	hstring AboutPage::ShowLicense()
	{
		return _showLicense;
	}

	hstring AboutPage::ShowLicenseToolTip()
	{
		return _showLicenseToolTip;
	}
}