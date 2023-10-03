#include "pch.h"
#include "AppWindowHelper.h"

/// <summary>
/// 初始化 AppWindow 窗口辅助类
/// </summary>
void InitializeAppWindow(HWND hwnd)
{
	if (appWindow == nullptr)
	{
		winrt::Microsoft::UI::WindowId windowId = winrt::Microsoft::UI::GetWindowIdFromWindow(hwnd);
		appWindow = winrt::Microsoft::UI::Windowing::AppWindow::GetFromWindowId(windowId);
	}
}

/// <summary>
/// 扩展标题栏，替换默认窗口标题栏
/// </summary>
bool ExtendsContentToTitleBar(bool value)
{
	if (appWindow != nullptr)
	{
		winrt::Microsoft::UI::Windowing::AppWindowTitleBar titleBar = appWindow.TitleBar();
		titleBar.ExtendsContentIntoTitleBar(value);
		titleBar.ButtonBackgroundColor(winrt::Windows::UI::Colors::Transparent());
		titleBar.InactiveBackgroundColor(winrt::Windows::UI::Colors::Transparent());

		return titleBar.ExtendsContentIntoTitleBar();
	}
	else
	{
		return false;
	}
}

/// <summary>
/// 设置标题栏按钮的主题颜色
/// </summary>
void SetWindowTitleBarColor(winrt::Microsoft::UI::Xaml::ElementTheme theme)
{
	if (appWindow != nullptr)
	{
		winrt::Microsoft::UI::Windowing::AppWindowTitleBar titleBar = appWindow.TitleBar();

		titleBar.BackgroundColor(winrt::Windows::UI::Colors::Transparent());
		titleBar.ForegroundColor(winrt::Windows::UI::Colors::Transparent());
		titleBar.InactiveBackgroundColor(winrt::Windows::UI::Colors::Transparent());
		titleBar.InactiveForegroundColor(winrt::Windows::UI::Colors::Transparent());

		if (theme == winrt::Microsoft::UI::Xaml::ElementTheme::Light)
		{
			titleBar.ButtonBackgroundColor(winrt::Windows::UI::Colors::Transparent());
			titleBar.ButtonForegroundColor(winrt::Windows::UI::Colors::Black());
			titleBar.ButtonHoverBackgroundColor(winrt::Windows::UI::ColorHelper::FromArgb(20, 0, 0, 0));
			titleBar.ButtonHoverForegroundColor(winrt::Windows::UI::Colors::Black());
			titleBar.ButtonPressedBackgroundColor(winrt::Windows::UI::ColorHelper::FromArgb(30, 0, 0, 0));
			titleBar.ButtonPressedForegroundColor(winrt::Windows::UI::Colors::Black());
			titleBar.ButtonInactiveBackgroundColor(winrt::Windows::UI::Colors::Transparent());
			titleBar.ButtonInactiveForegroundColor(winrt::Windows::UI::Colors::Black());
		}
		else
		{
			titleBar.ButtonBackgroundColor(winrt::Windows::UI::Colors::Transparent());
			titleBar.ButtonForegroundColor(winrt::Windows::UI::Colors::White());
			titleBar.ButtonHoverBackgroundColor(winrt::Windows::UI::ColorHelper::FromArgb(20, 255, 255, 255));
			titleBar.ButtonHoverForegroundColor(winrt::Windows::UI::Colors::White());
			titleBar.ButtonPressedBackgroundColor(winrt::Windows::UI::ColorHelper::FromArgb(30, 255, 255, 255));
			titleBar.ButtonPressedForegroundColor(winrt::Windows::UI::Colors::White());
			titleBar.ButtonInactiveBackgroundColor(winrt::Windows::UI::Colors::Transparent());
			titleBar.ButtonInactiveForegroundColor(winrt::Windows::UI::Colors::White());
		}
	}
}

/// <summary>
/// 卸载 AppWindow 窗口辅助类
/// </summary>
void UnInitializeAppWindow()
{
	appWindow = nullptr;
}