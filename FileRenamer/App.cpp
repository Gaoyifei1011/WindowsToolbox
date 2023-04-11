#include "pch.h"
#include "App.h"
#include "MileWindow.h"

namespace winrt::FileRenamer::implementation
{
	App::App()
	{
		MileXamlGlobalInitialize();
		InitializeComponent();
	}

	/// <summary>
	/// 启动应用
	/// </summary>
	void App::Run(HINSTANCE hInstance, int nShowCmd)
	{
		App::MainWindow.Content(winrt::make<winrt::FileRenamer::implementation::MainPage>());
		App::MainWindow.Position = { CW_USEDEFAULT,0 };
		App::MainWindow.Size = { CW_USEDEFAULT,0 };
		App::MainWindow.MinWindowSize = { 600,768 };
		App::MainWindow.MaxWindowSize = { 0,0 };
		App::MainWindow.InitializeWindow(hInstance);
		App::MainWindow.Activate(nShowCmd);
	}

	/// <summary>
	/// 关闭应用并释放所有资源
	/// </summary>
	void App::CloseApp()
	{
		MileXamlGlobalUninitialize();
		Exit();
	}
}