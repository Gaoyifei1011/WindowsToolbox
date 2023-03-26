#pragma once

#include <string>
#include <Windows.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.UI.Xaml.h>

#include "MileWindow.h"
#include "MainPage.xaml.h"

using namespace std;
using namespace winrt;
using namespace winrt::Windows::Graphics;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Controls;

MileWindow::MileWindow(string title, UIElement content, PointInt32 position, PointInt32 size) :_title(title), _content(content)
{
	if (title == "")
	{
		MileWindow::Title(title);
	}
	else
	{
		MileWindow::Title("");
	}

	if (content != nullptr)
	{
		MileWindow::Content(content);
	}
	else
	{
		winrt::FileRenamer::MainPage control = winrt::make<winrt::FileRenamer::implementation::MainPage>();
		MileWindow::Content(control);
	}

	MileWindow::Position.X = position.X;
	MileWindow::Position.Y = position.Y;

	MileWindow::Size.X = size.X;
	MileWindow::Size.Y = size.Y;
}

bool MileWindow::IsWindowCreated()
{
	return _isWindowCreated;
}

void MileWindow::IsWindowCreated(bool value)
{
	_isWindowCreated = value;
}

string MileWindow::Title()
{
	return _title;
}

void MileWindow::Title(string value)
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

UIElement MileWindow::Content()
{
	return _content;
}

void MileWindow::Content(UIElement value)
{
	_content = value;
}

/// <summary>
/// 初始化应用窗口
/// </summary>
void MileWindow::InitializeWindow(HINSTANCE hInstance)
{
	_handle = CreateWindowExW(
		WS_EX_LEFT,
		L"Mile.Xaml.ContentWindow",
		L"FileRenamer",
		WS_OVERLAPPEDWINDOW,
		MileWindow::Position.X,
		MileWindow::Position.Y,
		MileWindow::Size.X,
		MileWindow::Size.Y,
		nullptr,
		nullptr,
		hInstance,
		get_abi(MileWindow::Content()));

	if (_handle == nullptr)
	{
		throw "HelperResources/WindowHandleInitializeFailed";
	}
	else
	{
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
	}

	MSG Message;

	while (::GetMessage(&Message, nullptr, WM_NULL, WM_NULL))
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