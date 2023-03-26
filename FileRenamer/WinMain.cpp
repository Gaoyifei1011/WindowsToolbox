#pragma once

#include <Windows.h>
#include <WinMain.h>

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

using namespace winrt;
using namespace winrt::FileRenamer::implementation;

winrt::com_ptr<App> ApplicationRoot;
HWND WindowHandle;

/// <summary>
/// 文件重命名工具
/// </summary>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);
	OutputDebugString(L"Hello App1");
	winrt::init_apartment(apartment_type::single_threaded);
	OutputDebugString(L"Hello App2");
	ApplicationRoot = winrt::make_self<App>();
	OutputDebugString(L"Hello App11");
	ApplicationRoot->Run(hInstance, nShowCmd);
	return 0;
}