#pragma once

#include <winrt/base.h>

#include "global.h"
#include "RestartAppsDialog.g.h"
#include "ViewModels/Dialogs/Settings/RestartAppsViewModel.h"

namespace winrt::FileRenamer::implementation
{
	struct RestartAppsDialog : RestartAppsDialogT<RestartAppsDialog>
	{
	public:
		RestartAppsDialog();
		winrt::FileRenamer::RestartAppsViewModel ViewModel();

		winrt::hstring Restart();
		winrt::hstring RestartContent();
		winrt::hstring OK();
		winrt::hstring Cancel();

	private:
		winrt::FileRenamer::RestartAppsViewModel _viewModel;

		winrt::hstring _restart;
		winrt::hstring _restartContent;
		winrt::hstring _ok;
		winrt::hstring _cancel;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct RestartAppsDialog : RestartAppsDialogT<RestartAppsDialog, implementation::RestartAppsDialog>
	{
	};
}
