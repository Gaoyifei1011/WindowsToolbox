#pragma once

#include <winrt/base.h>

#include "global.h"
#include "AboutPage.g.h"
#include "ViewModels/Pages/AboutViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct AboutPage : AboutPageT<AboutPage>
	{
	public:
		AboutPage();

		winrt::FileRenamer::AboutViewModel ViewModel();

		winrt::hstring BriefIntroduction();
		winrt::hstring CreateDesktopShortcut();
		winrt::hstring CreateDesktopShortcutToolTip();
		winrt::hstring PinToStartScreen();
		winrt::hstring PinToStartScreenToolTip();
		winrt::hstring PinToTaskbar();
		winrt::hstring PinToTaskbarToolTip();
		winrt::hstring Precaution();
		winrt::hstring QuickOperation();
		winrt::hstring Reference();
		winrt::hstring ReferenceTitle();
		winrt::hstring SettingsHelp();
		winrt::hstring ShowLicense();
		winrt::hstring ShowLicenseToolTip();
		winrt::hstring ShowReleaseNotes();
		winrt::hstring ShowReleaseNotesToolTip();
		winrt::hstring Thanks();
		winrt::hstring ThanksTitle();
		winrt::hstring Title();
		winrt::hstring UpdateAndLicensing();
		winrt::hstring UseInstruction();

	private:
		winrt::FileRenamer::AboutViewModel _viewModel;

		winrt::hstring _briefIntroduction;
		winrt::hstring _createDesktopShortcut;
		winrt::hstring _createDesktopShortcutToolTip;
		winrt::hstring _pinToStartScreen;
		winrt::hstring _pinToStartScreenToolTip;
		winrt::hstring _pinToTaskbar;
		winrt::hstring _pinToTaskbarToolTip;
		winrt::hstring _precaution;
		winrt::hstring _quickOperation;
		winrt::hstring _reference;
		winrt::hstring _referenceTitle;
		winrt::hstring _settingsHelp;
		winrt::hstring _showLicense;
		winrt::hstring _showLicenseToolTip;
		winrt::hstring _showReleaseNotes;
		winrt::hstring _showReleaseNotesToolTip;
		winrt::hstring _thanks;
		winrt::hstring _thanksTitle;
		winrt::hstring _title;
		winrt::hstring _updateAndLicensing;
		winrt::hstring _useInstruction;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AboutPage : AboutPageT<AboutPage, implementation::AboutPage>
	{
	};
}
