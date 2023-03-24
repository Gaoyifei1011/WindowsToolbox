#include <Windows.h>
#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

/// <summary>
/// 文件重命名工具
/// </summary>
int WINAPI wWinMain(
	_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR lpCmdLine,
	_In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	winrt::init_apartment(winrt::apartment_type::single_threaded);

	winrt::com_ptr<winrt::FileRenamer::implementation::App> app =
		winrt::make_self<winrt::FileRenamer::implementation::App>();

	winrt::FileRenamer::MainPage XamlWindowContent =
		winrt::make<winrt::FileRenamer::implementation::MainPage>();

	HWND WindowHandle = ::CreateWindowExW(
		WS_EX_LEFT,
		L"Mile.Xaml.ContentWindow",
		L"FileRenamer",
		WS_OVERLAPPEDWINDOW,
		200,
		200,
		600,
		600,
		nullptr,
		nullptr,
		hInstance,
		winrt::get_abi(XamlWindowContent));
	if (!WindowHandle)
	{
		return -1;
	}

	//SetWindowLong(WindowHandle, GWL_STYLE, GetWindowLong(WindowHandle, GWL_STYLE) & ~WS_CAPTION);
	//SetWindowPos(WindowHandle, NULL, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_DRAWFRAME);
	::ShowWindow(WindowHandle, nShowCmd);
	::UpdateWindow(WindowHandle);

	MSG Message;

	while (::GetMessageW(&Message, nullptr, 0, 0))
	{
		// Workaround for capturing Alt+F4 in applications with XAML Islands.
		// Reference: https://github.com/microsoft/microsoft-ui-xaml/issues/2408
		if (Message.message == WM_SYSKEYDOWN && Message.wParam == VK_F4)
		{
			::SendMessageW(
				::GetAncestor(Message.hwnd, GA_ROOT),
				Message.message,
				Message.wParam,
				Message.lParam);

			continue;
		}

		::TranslateMessage(&Message);
		::DispatchMessageW(&Message);
	}

	app->Close();

	return static_cast<int>(Message.wParam);
}