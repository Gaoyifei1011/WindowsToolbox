#pragma once

#include <Mile.Xaml.h>

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

using namespace winrt;
using namespace FileRenamer;
using namespace FileRenamer::implementation;

namespace winrt::FileRenamer::implementation
{
	App::App()
	{
		OutputDebugString(L"Hello App12");
		MileXamlGlobalInitialize();
		OutputDebugString(L"Hello App13");
	}

	MileWindow App::MainWindow()
	{
		return _mainWindow;
	}

	/// <summary>
	/// 启动应用
	/// </summary>
	void App::Run(HINSTANCE hInstance, int nShowCmd)
	{
		OutputDebugString(L"Hello App3");
		App::MainWindow().Content(make<MainPage>());
		OutputDebugString(L"Hello App4");
		App::MainWindow().Position = { 0,0 };
		App::MainWindow().Size = { 0,0 };
		App::MainWindow().InitializeWindow(hInstance);
		App::MainWindow().Activate(nShowCmd);
		OutputDebugString(L"Hello App5");
	}

	/// <summary>
	/// 关闭应用并释放所有资源
	/// </summary>
	void App::CloseApp()
	{
		::MileXamlGlobalUninitialize();
		Exit();
	}
}