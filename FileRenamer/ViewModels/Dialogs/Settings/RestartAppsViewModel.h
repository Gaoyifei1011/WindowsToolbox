#pragma once

#include <winrt/Windows.ApplicationModel.Core.h>
#include <winrt/Windows.Storage.h>

#include "Extensions/Command/RelayCommand.h"
#include "MileWindow.h"
#include "RestartAppsViewModel.g.h"

namespace winrt
{
	namespace WinrtApplicationModel = Windows::ApplicationModel;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInput = Windows::UI::Xaml::Input;
	namespace WinrtStorage = Windows::Storage;
}

namespace winrt::FileRenamer::implementation
{
	struct RestartAppsViewModel : RestartAppsViewModelT<RestartAppsViewModel>
	{
	public:
		RestartAppsViewModel();

		winrt::WinrtInput::ICommand RestartAppsCommand();
		winrt::WinrtInput::ICommand CloseDialogCommand();

	private:
		winrt::WinrtInput::ICommand _restartAppsCommand;
		winrt::WinrtInput::ICommand _closeDialogCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct RestartAppsViewModel : RestartAppsViewModelT<RestartAppsViewModel, implementation::RestartAppsViewModel>
	{
	};
}
