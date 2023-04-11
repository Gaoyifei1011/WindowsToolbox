#pragma once

#include "winrt/base.h"
#include "AboutPage.g.h"
#include "WinMain.h"
#include "ViewModels/Pages/AboutViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct AboutPage : AboutPageT<AboutPage>
	{
	public:
		AboutPage();

		winrt::FileRenamer::AboutViewModel ViewModel();

		winrt::hstring Title();
		winrt::hstring BriefIntroduction();
		winrt::hstring Reference();
		winrt::hstring UseInstruction();
		winrt::hstring Precaution();
		winrt::hstring SettingsHelp();
		winrt::hstring Thanks();
		winrt::hstring QuickOperation();
		winrt::hstring CreateDesktopShortcut();
		winrt::hstring CreateDesktopShortcutToolTip();
		winrt::hstring PinToStartScreen();
		winrt::hstring PinToStartScreenToolTip();
		winrt::hstring PinToTaskbar();
		winrt::hstring PinToTaskbarToolTip();
		winrt::hstring UpdateAndLicensing();
		winrt::hstring ShowReleaseNotes();
		winrt::hstring ShowReleaseNotesToolTip();
		winrt::hstring ShowLicense();
		winrt::hstring ShowLicenseToolTip();

	private:
		winrt::FileRenamer::AboutViewModel _viewModel;

		winrt::hstring _title;
		winrt::hstring _briefIntroduction;
		winrt::hstring _reference;
		winrt::hstring _useInstruction;
		winrt::hstring _precaution;
		winrt::hstring _settingsHelp;
		winrt::hstring _thanks;
		winrt::hstring _quickOperation;
		winrt::hstring _createDesktopShortcut;
		winrt::hstring _createDesktopShortcutToolTip;
		winrt::hstring _pinToStartScreen;
		winrt::hstring _pinToStartScreenToolTip;
		winrt::hstring _pinToTaskbar;
		winrt::hstring _pinToTaskbarToolTip;
		winrt::hstring _updateAndLicensing;
		winrt::hstring _showReleaseNotes;
		winrt::hstring _showReleaseNotesToolTip;
		winrt::hstring _showLicense;
		winrt::hstring _showLicenseToolTip;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AboutPage : AboutPageT<AboutPage, implementation::AboutPage>
	{
	};
}
