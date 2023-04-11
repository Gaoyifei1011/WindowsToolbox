#pragma once

#include <string>
#include <Windows.h>
#include <WinMain.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.UI.Xaml.h>

#include "Helpers/Root/DPICalcHelper.h"
#include "MileWindow.h"
#include "MainPage.h"

WNDPROC MileOldWndProc = 0;
MileWindow* MileWindow::_current = nullptr;
LRESULT CALLBACK MileNewWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

MileWindow::MileWindow()
{
	MileWindow::Position = { 0,0 };
	MileWindow::Size = { 0,0 };
	MileWindow::MinWindowSize = { 0,0 };
	MileWindow::MaxWindowSize = { 0,0 };
	MileWindow::_handle = NULL;
	MileWindow::_isWindowCreated = false;
}

bool MileWindow::IsWindowCreated()
{
	return _isWindowCreated;
}
void MileWindow::IsWindowCreated(bool value)
{
	_isWindowCreated = value;
}

std::string MileWindow::Title()
{
	return _title;
}
void MileWindow::Title(std::string value)
{
	_title = value;
}

HWND MileWindow::Handle()
{
	return _handle;
}
void MileWindow::Handle(HWND value)
{
	_handle = value;
}

winrt::WinrtXaml::UIElement MileWindow::Content()
{
	return _content;
}
void MileWindow::Content(winrt::WinrtXaml::UIElement value)
{
	_content = value;
}

MileWindow* MileWindow::Current()
{
	return _current;
}

/// <summary>
/// 初始化应用窗口
/// </summary>
void MileWindow::InitializeWindow(HINSTANCE hInstance)
{
	winrt::hstring AppTitle = AppResourcesService.GetLocalized(L"Resources/AppDisplayName");

	HWND hwnd = CreateWindowExW(
		WS_EX_LEFT,
		L"Mile.Xaml.ContentWindow",
		AppTitle.c_str(),
		WS_OVERLAPPEDWINDOW,
		MileWindow::Position.X,
		MileWindow::Position.Y,
		MileWindow::Size.X,
		MileWindow::Size.Y,
		nullptr,
		nullptr,
		hInstance,
		winrt::get_abi(MileWindow::Content()));

	MileWindow::Handle(hwnd);
	if (MileWindow::Handle() == nullptr)
	{
		throw AppResourcesService.GetLocalized(L"Resources/WindowHandleInitializeFailed");
	}
	else
	{
		MileOldWndProc = (WNDPROC)SetWindowLongPtr(MileWindow::Handle(), GWLP_WNDPROC, (LONG_PTR)MileNewWndProc);
		MileWindow::IsWindowCreated(true);
	}
}

/// <summary>
/// 激活窗口
/// </summary>
void MileWindow::Activate(int nShowCmd)
{
	if (MileWindow::IsWindowCreated() == true)
	{
		ShowWindow(MileWindow::Handle(), nShowCmd);
		UpdateWindow(MileWindow::Handle());
		MileWindow::_current = this;
		MileWindow::SetAppIcon();
	}

	MSG Message;

	while (GetMessage(&Message, nullptr, WM_NULL, WM_NULL))
	{
		// Workaround for capturing Alt+F4 in applications with XAML Islands.
		// Reference: https://github.com/microsoft/microsoft-ui-xaml/issues/2408
		if (Message.message == WM_SYSKEYDOWN && Message.wParam == VK_F4)
		{
			PostMessage(GetAncestor(Message.hwnd, GA_ROOT), Message.message, Message.wParam, Message.lParam);
			continue;
		}

		TranslateMessage(&Message);
		DispatchMessage(&Message);
	}
}

/// <summary>
/// 设置应用窗口图标
/// </summary>
void MileWindow::SetAppIcon()
{
	TCHAR szFullPath[MAX_PATH];
	ZeroMemory(szFullPath, MAX_PATH);
	wsprintf(szFullPath, L"%s\\%s", winrt::WinrtApplicationModel::Package::Current().InstalledLocation().Path().c_str(), L"FileRenamer.exe");
	HICON AppIcon = MileWindow::LoadLocalExeIcon(szFullPath);
	SendMessage(Current()->Handle(), WM_SETICON, ICON_BIG, (LPARAM)AppIcon);
	SendMessage(Current()->Handle(), WM_SETICON, ICON_SMALL, (LPARAM)AppIcon);
}

/// <summary>
/// 加载应用窗口图标
/// </summary>
HICON MileWindow::LoadLocalExeIcon(LPCWSTR exeFile)
{
	// 选中文件中的图标总数
	int iconTotalCount = PrivateExtractIcons(exeFile, 0, 0, 0, nullptr, nullptr, 0, 0);

	// 用于接收获取到的图标指针
	HICON* hIcons = new HICON[iconTotalCount];

	// 对应的图标id
	UINT* ids = new UINT[iconTotalCount];

	// 成功获取到的图标个数
	int successCount = PrivateExtractIcons(exeFile, 0, 16, 16, hIcons, ids, iconTotalCount, 0);

	// FileRenamer.exe 应用程序只有一个图标，返回该应用程序的图标句柄
	if (successCount >= 1 && hIcons[0] != nullptr)
	{
		return hIcons[0];
	}
	else
	{
		return nullptr;
	}
}

/// <summary>
/// 窗口消息处理
/// </summary>
LRESULT CALLBACK MileNewWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch (msg)
	{
		// 窗口大小发生更改时的消息
	case WM_GETMINMAXINFO:
	{
		MINMAXINFO* minMaxInfo = (MINMAXINFO*)lParam;
		if (MileWindow::Current()->MinWindowSize.X >= 0)
		{
			minMaxInfo->ptMinTrackSize.x = DPICalcHelper::ConvertEpxToPixel(MileWindow::Current()->Handle(), MileWindow::Current()->MinWindowSize.X);
		}
		if (MileWindow::Current()->MinWindowSize.Y >= 0)
		{
			minMaxInfo->ptMinTrackSize.y = DPICalcHelper::ConvertEpxToPixel(MileWindow::Current()->Handle(), MileWindow::Current()->MinWindowSize.Y);
		}
		if (MileWindow::Current()->MaxWindowSize.X > 0)
		{
			minMaxInfo->ptMinTrackSize.x = DPICalcHelper::ConvertEpxToPixel(MileWindow::Current()->Handle(), MileWindow::Current()->MaxWindowSize.X);
		}
		if (MileWindow::Current()->MaxWindowSize.Y > 0)
		{
			minMaxInfo->ptMinTrackSize.y = DPICalcHelper::ConvertEpxToPixel(MileWindow::Current()->Handle(), MileWindow::Current()->MaxWindowSize.Y);
		}
	}
	}

	return CallWindowProc(MileOldWndProc, hwnd, msg, wParam, lParam);
}