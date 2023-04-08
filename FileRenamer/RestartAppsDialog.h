#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "RestartAppsDialog.g.h"
#include "ViewModels/Dialogs/Settings/RestartAppsViewModel.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct RestartAppsDialog : RestartAppsDialogT<RestartAppsDialog>
	{
	public:
		RestartAppsDialog();
		FileRenamer::RestartAppsViewModel ViewModel();

		hstring Restart();
		hstring RestartContent();
		hstring OK();
		hstring Cancel();

	private:
		FileRenamer::RestartAppsViewModel _viewModel;

		hstring _restart;
		hstring _restartContent;
		hstring _ok;
		hstring _cancel;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct RestartAppsDialog : RestartAppsDialogT<RestartAppsDialog, implementation::RestartAppsDialog>
	{
	};
}
