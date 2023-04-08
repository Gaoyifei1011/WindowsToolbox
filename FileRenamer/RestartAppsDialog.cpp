#include "pch.h"
#include "RestartAppsDialog.h"
#include "RestartAppsDialog.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	RestartAppsDialog::RestartAppsDialog()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::RestartAppsViewModel>();

		_restart = AppResourcesService.GetLocalized(L"Dialog/Restart");
		_restartContent = AppResourcesService.GetLocalized(L"Dialog/RestartContent");
		_ok = AppResourcesService.GetLocalized(L"Dialog/OK");
		_cancel = AppResourcesService.GetLocalized(L"Dialog/Cancel");
	};

	FileRenamer::RestartAppsViewModel RestartAppsDialog::ViewModel()
	{
		return _viewModel;
	}

	hstring RestartAppsDialog::Restart()
	{
		return _restart;
	}

	hstring RestartAppsDialog::RestartContent()
	{
		return _restartContent;
	}

	hstring RestartAppsDialog::OK()
	{
		return _ok;
	}

	hstring RestartAppsDialog::Cancel()
	{
		return _cancel;
	}
}