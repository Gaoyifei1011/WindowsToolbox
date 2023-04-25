#include "pch.h"
#include "RestartAppsDialog.h"
#include "RestartAppsDialog.g.cpp"

namespace winrt::FileRenamer::implementation
{
	RestartAppsDialog::RestartAppsDialog()
	{
		InitializeComponent();

		_viewModel = winrt::make<winrt::FileRenamer::implementation::RestartAppsViewModel>();

		_restart = AppResourceService.GetLocalized(L"Dialog/Restart");
		_restartContent = AppResourceService.GetLocalized(L"Dialog/RestartContent");
		_ok = AppResourceService.GetLocalized(L"Dialog/OK");
		_cancel = AppResourceService.GetLocalized(L"Dialog/Cancel");
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