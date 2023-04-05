#pragma once

#include "winrt/Windows.UI.Xaml.Controls.h"
#include "pch.h"
#include "MileWindow.h"
#include "RestartContentDialog.h"
#include "SettingsViewModel.h"
#include "SettingsViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::FileRenamer::implementation
{
	SettingsViewModel::SettingsViewModel()
	{
		_restartCommand = make<RelayCommand>([this](IInspectable parameter)
			{
				FileRenamer::RestartContentDialog dialog;
				dialog.Title(box_value(L"title"));
				dialog.Content(box_value(L"content"));
				dialog.PrimaryButtonText(L"primary");
				dialog.CloseButtonText(L"close");
				dialog.XamlRoot(MileWindow::Current()->Content().XamlRoot());
				auto result = dialog.ShowAsync().GetResults();
			});
	};

	/// <summary>
	/// 打开重启应用确认的窗口对话框
	/// </summary>
	ICommand SettingsViewModel::RestartCommand()
	{
		return _restartCommand;
	}
}