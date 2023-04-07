#include "pch.h"
#include "winrt/Windows.ApplicationModel.Core.h"
#include "MileWindow.h"
#include "RestartAppsViewModel.h"
#include "RestartAppsViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::FileRenamer::implementation
{
	void RestartApps();

	RestartAppsViewModel::RestartAppsViewModel()
	{
		_restartAppsCommand = make<RelayCommand>([this](IInspectable dialog)
			{
				if (dialog != nullptr)
				{
					ContentDialog restartAppsDialog = unbox_value<ContentDialog>(dialog);
					restartAppsDialog.Hide();
					RestartApps();
				}
			});

		_closeDialogCommand = make<RelayCommand>([this](IInspectable dialog)
			{
				if (dialog != nullptr)
				{
					ContentDialog restartAppsDialog = unbox_value<ContentDialog>(dialog);
					restartAppsDialog.Hide();
				}
			});
	};

	/// <summary>
	/// 重启应用
	/// </summary>
	ICommand RestartAppsViewModel::RestartAppsCommand()
	{
		return _restartAppsCommand;
	}

	/// <summary>
	/// 取消重启应用
	/// </summary>
	ICommand RestartAppsViewModel::CloseDialogCommand()
	{
		return _closeDialogCommand;
	}

	/// <summary>
	/// 重启应用
	/// </summary>
	void RestartApps()
	{
		// 先隐藏窗口
		ShowWindow(MileWindow::Current()->Handle(), SW_HIDE);

		// 启动新的应用实例
		STARTUPINFO FileRenamerStartupInfo;
		GetStartupInfo(&FileRenamerStartupInfo);
		FileRenamerStartupInfo.lpReserved = NULL;
		FileRenamerStartupInfo.lpDesktop = NULL;
		FileRenamerStartupInfo.lpTitle = NULL;
		FileRenamerStartupInfo.dwX = 0;
		FileRenamerStartupInfo.dwY = 0;
		FileRenamerStartupInfo.dwXSize = 0;
		FileRenamerStartupInfo.dwYSize = 0;
		FileRenamerStartupInfo.dwXCountChars = 500;
		FileRenamerStartupInfo.dwYCountChars = 500;
		FileRenamerStartupInfo.dwFlags = STARTF_USESHOWWINDOW;
		FileRenamerStartupInfo.wShowWindow = 1;
		FileRenamerStartupInfo.cbReserved2 = 0;
		FileRenamerStartupInfo.lpReserved2 = NULL;
		FileRenamerStartupInfo.cb = sizeof(STARTUPINFO);

		PROCESS_INFORMATION FileRenamerProcessInformation;

		TCHAR szFullPath[MAX_PATH];
		ZeroMemory(szFullPath, MAX_PATH);
		GetModuleFileName(NULL, szFullPath, MAX_PATH);
		wsprintf(szFullPath, L"%s %s", szFullPath, L"Restart");
		bool CreateResult = CreateProcess(NULL, szFullPath, 0, 0, false, 0, NULL, NULL, &FileRenamerStartupInfo, &FileRenamerProcessInformation);
		if (CreateResult)
		{
			CloseHandle(FileRenamerProcessInformation.hProcess);
			CloseHandle(FileRenamerProcessInformation.hThread);
		}

		// 获取当前进程ID并关闭进程
		DWORD ProcessId = GetCurrentProcessId();
		HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, false, ProcessId);
		if (hProcess != nullptr)
		{
			TerminateProcess(hProcess, 0);
			CloseHandle(hProcess);
		}
	}
}