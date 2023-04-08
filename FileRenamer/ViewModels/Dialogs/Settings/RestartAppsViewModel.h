#pragma once

#include "Extensions/Command/RelayCommand.h"
#include "RestartAppsViewModel.g.h"

namespace winrt::FileRenamer::implementation
{
	struct RestartAppsViewModel : RestartAppsViewModelT<RestartAppsViewModel>
	{
	public:
		RestartAppsViewModel();

		ICommand RestartAppsCommand();
		ICommand CloseDialogCommand();

	private:
		ICommand _restartAppsCommand;
		ICommand _closeDialogCommand;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct RestartAppsViewModel : RestartAppsViewModelT<RestartAppsViewModel, implementation::RestartAppsViewModel>
	{
	};
}
