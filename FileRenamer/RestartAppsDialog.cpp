#include "pch.h"
#include "RestartAppsDialog.h"
#include "RestartAppsDialog.g.cpp"

namespace winrt::FileRenamer::implementation
{
	RestartAppsDialog::RestartAppsDialog()
	{
		InitializeComponent();

		_viewModel = winrt::make<winrt::FileRenamer::implementation::RestartAppsViewModel>();

		_restart = AppResourcesService.GetLocalized(L"Dialog/Restart");
		_restartContent = AppResourcesService.GetLocalized(L"Dialog/RestartContent");
		_ok = AppResourcesService.GetLocalized(L"Dialog/OK");
		_cancel = AppResourcesService.GetLocalized(L"Dialog/Cancel");
	};

	winrt::FileRenamer::RestartAppsViewModel RestartAppsDialog::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring RestartAppsDialog::Restart()
	{
		return _restart;
	}

	winrt::hstring RestartAppsDialog::RestartContent()
	{
		return _restartContent;
	}

	winrt::hstring RestartAppsDialog::OK()
	{
		return _ok;
	}

	winrt::hstring RestartAppsDialog::Cancel()
	{
		return _cancel;
	}
}