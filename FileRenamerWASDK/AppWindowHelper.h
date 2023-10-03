#pragma once
#include "pch.h"

winrt::Microsoft::UI::Windowing::AppWindow appWindow{ nullptr };

extern "C" _declspec(dllexport) void InitializeAppWindow(HWND hwnd);

extern "C" _declspec(dllexport) bool ExtendsContentToTitleBar(bool value);

extern "C" _declspec(dllexport) void SetWindowTitleBarColor(winrt::Microsoft::UI::Xaml::ElementTheme theme);

extern "C" _declspec(dllexport) void UnInitializeAppWindow();