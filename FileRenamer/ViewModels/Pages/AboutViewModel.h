#pragma once

#include <windows.h>
#include <shellapi.h>
#include <ShlObj.h>
#pragma  comment(lib, "shell32.lib")
#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.Core.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.StartScreen.h>
#include <winrt/Windows.System.h>
#include <Extensions/Command/RelayCommand.h>
#include <AboutViewModel.g.h>

#include "global.h"
#include "Models/About/ReferenceKeyValuePairModel.h"
#include "Models/About/ThanksKeyValuePairModel.h"

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

		winrt::WinrtInput::ICommand CreateDesktopShortcutCommand();
		winrt::WinrtInput::ICommand PinToStartScreenCommand();
		winrt::WinrtInput::ICommand PinToTaskbarCommand();
		winrt::WinrtInput::ICommand ShowReleaseNotesCommand();
		winrt::WinrtInput::ICommand ShowLicenseCommand();

		winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ReferenceKeyValuePairModel> ReferenceDict();
		winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThanksKeyValuePairModel> ThanksDict();

	private:
		winrt::WinrtInput::ICommand _createDesktopShortcutCommand;
		winrt::WinrtInput::ICommand _pinToStartScreenCommand;
		winrt::WinrtInput::ICommand _pinToTaskbarCommand;
		winrt::WinrtInput::ICommand _showReleaseNotesCommand;
		winrt::WinrtInput::ICommand _showLicenseCommand;

		winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ReferenceKeyValuePairModel> _referenceDict{ winrt::single_threaded_observable_vector<winrt::FileRenamer::ReferenceKeyValuePairModel>() };
		winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThanksKeyValuePairModel> _thanksDict{ winrt::single_threaded_observable_vector<winrt::FileRenamer::ThanksKeyValuePairModel>() };
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct AboutViewModel : AboutViewModelT<AboutViewModel, implementation::AboutViewModel>
	{
	};
}
