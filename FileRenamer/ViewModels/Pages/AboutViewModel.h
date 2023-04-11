#pragma once

#include <windows.h>
#include <shellapi.h>
#include <ShlObj.h>
#pragma  comment(lib, "shell32.lib")
#include <WinMain.h>

#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.Core.h>
#include <winrt/Windows.UI.StartScreen.h>
#include <winrt/Windows.System.h>
#include <Extensions/Command/RelayCommand.h>
#include <AboutViewModel.g.h>

namespace winrt
{
	namespace WinrtApplicationModel = Windows::ApplicationModel;
	namespace WinrtApplicationModelCore = Windows::ApplicationModel::Core;
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtStartScreen = Windows::UI::StartScreen;
	namespace WinrtSystem = Windows::System;
}

namespace winrt::FileRenamer::implementation
{
	struct AboutViewModel : AboutViewModelT<AboutViewModel>
	{
	public:
		AboutViewModel();

		WinrtInput::ICommand CreateDesktopShortcutCommand();
		WinrtInput::ICommand PinToStartScreenCommand();
		WinrtInput::ICommand PinToTaskbarCommand();
		WinrtInput::ICommand ShowReleaseNotesCommand();
		WinrtInput::ICommand ShowLicenseCommand();

	private:
		WinrtInput::ICommand _createDesktopShortcutCommand;
		WinrtInput::ICommand _pinToStartScreenCommand;
		WinrtInput::ICommand _pinToTaskbarCommand;
		WinrtInput::ICommand _showReleaseNotesCommand;
		WinrtInput::ICommand _showLicenseCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AboutViewModel : AboutViewModelT<AboutViewModel, implementation::AboutViewModel>
	{
	};
}
