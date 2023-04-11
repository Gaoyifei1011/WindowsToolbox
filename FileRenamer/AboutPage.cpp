#include "pch.h"
#include "AboutPage.h"
#include "AboutPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AboutPage::AboutPage()
	{
		InitializeComponent();

		_viewModel = winrt::make<winrt::FileRenamer::implementation::AboutViewModel>();

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

	winrt::FileRenamer::AboutViewModel AboutPage::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring AboutPage::Title()
	{
		return _title;
	}

	winrt::hstring AboutPage::BriefIntroduction()
	{
		return _briefIntroduction;
	}

	winrt::hstring AboutPage::Reference()
	{
		return _reference;
	}

	winrt::hstring AboutPage::UseInstruction()
	{
		return _useInstruction;
	}

	winrt::hstring AboutPage::Precaution()
	{
		return _precaution;
	}

	winrt::hstring AboutPage::SettingsHelp()
	{
		return _settingsHelp;
	}

	winrt::hstring AboutPage::Thanks()
	{
		return _thanks;
	}

	winrt::hstring AboutPage::QuickOperation()
	{
		return _quickOperation;
	}

	winrt::hstring AboutPage::CreateDesktopShortcut()
	{
		return _createDesktopShortcut;
	}

	winrt::hstring AboutPage::CreateDesktopShortcutToolTip()
	{
		return _createDesktopShortcutToolTip;
	}

	winrt::hstring AboutPage::PinToStartScreen()
	{
		return _pinToStartScreen;
	}

	winrt::hstring AboutPage::PinToStartScreenToolTip()
	{
		return _pinToStartScreenToolTip;
	}

	winrt::hstring AboutPage::PinToTaskbar()
	{
		return _pinToTaskbar;
	}

	winrt::hstring AboutPage::PinToTaskbarToolTip()
	{
		return _pinToTaskbarToolTip;
	}

	winrt::hstring AboutPage::UpdateAndLicensing()
	{
		return _updateAndLicensing;
	}

	winrt::hstring AboutPage::ShowReleaseNotes()
	{
		return _showReleaseNotes;
	}

	winrt::hstring AboutPage::ShowReleaseNotesToolTip()
	{
		return _showReleaseNotesToolTip;
	}

	winrt::hstring AboutPage::ShowLicense()
	{
		return _showLicense;
	}

	winrt::hstring AboutPage::ShowLicenseToolTip()
	{
		return _showLicenseToolTip;
	}
}