#include "pch.h"
#include "AboutPage.h"
#include "AboutPage.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AboutPage::AboutPage()
	{
		InitializeComponent();

		_viewModel = winrt::make<winrt::FileRenamer::implementation::AboutViewModel>();

		_briefIntroduction = AppResourceService.GetLocalized(L"About/BriefIntroduction");
		_createDesktopShortcut = AppResourceService.GetLocalized(L"About/CreateDesktopShortcut");
		_createDesktopShortcutToolTip = AppResourceService.GetLocalized(L"About/CreateDesktopShortcutToolTip");
		_pinToStartScreen = AppResourceService.GetLocalized(L"About/PinToStartScreen");
		_pinToStartScreenToolTip = AppResourceService.GetLocalized(L"About/PinToStartScreenToolTip");
		_pinToTaskbar = AppResourceService.GetLocalized(L"About/PinToTaskbar");
		_pinToTaskbarToolTip = AppResourceService.GetLocalized(L"About/PinToTaskbarToolTip");
		_precaution = AppResourceService.GetLocalized(L"About/Precaution");
		_quickOperation = AppResourceService.GetLocalized(L"About/QuickOperation");
		_reference = AppResourceService.GetLocalized(L"About/Reference");
		_referenceTitle = AppResourceService.GetLocalized(L"About/ReferenceTitle");
		_settingsHelp = AppResourceService.GetLocalized(L"About/SettingsHelp");
		_showLicense = AppResourceService.GetLocalized(L"About/ShowLicense");
		_showLicenseToolTip = AppResourceService.GetLocalized(L"About/ShowLicenseToolTip");
		_showReleaseNotes = AppResourceService.GetLocalized(L"About/ShowReleaseNotes");
		_showReleaseNotesToolTip = AppResourceService.GetLocalized(L"About/ShowReleaseNotesToolTip");
		_useInstruction = AppResourceService.GetLocalized(L"About/UseInstruction");
		_thanks = AppResourceService.GetLocalized(L"About/Thanks");
		_thanksTitle = AppResourceService.GetLocalized(L"About/ThanksTitle");
		_title = AppResourceService.GetLocalized(L"About/Title");
		_updateAndLicensing = AppResourceService.GetLocalized(L"About/UpdateAndLicensing");
		_useInstruction = AppResourceService.GetLocalized(L"About/UseInstruction");
	};

	winrt::FileRenamer::AboutViewModel AboutPage::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring AboutPage::BriefIntroduction()
	{
		return _briefIntroduction;
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

	winrt::hstring AboutPage::Precaution()
	{
		return _precaution;
	}

	winrt::hstring AboutPage::QuickOperation()
	{
		return _quickOperation;
	}

	winrt::hstring AboutPage::Reference()
	{
		return _reference;
	}

	winrt::hstring AboutPage::ReferenceTitle()
	{
		return _referenceTitle;
	}

	winrt::hstring AboutPage::SettingsHelp()
	{
		return _settingsHelp;
	}

	winrt::hstring AboutPage::ShowLicense()
	{
		return _showLicense;
	}

	winrt::hstring AboutPage::ShowLicenseToolTip()
	{
		return _showLicenseToolTip;
	}

	winrt::hstring AboutPage::ShowReleaseNotes()
	{
		return _showReleaseNotes;
	}

	winrt::hstring AboutPage::ShowReleaseNotesToolTip()
	{
		return _showReleaseNotesToolTip;
	}

	winrt::hstring AboutPage::Thanks()
	{
		return _thanks;
	}

	winrt::hstring AboutPage::ThanksTitle()
	{
		return _thanksTitle;
	}

	winrt::hstring AboutPage::Title()
	{
		return _title;
	}

	winrt::hstring AboutPage::UpdateAndLicensing()
	{
		return _updateAndLicensing;
	}

	winrt::hstring AboutPage::UseInstruction()
	{
		return _useInstruction;
	}
}