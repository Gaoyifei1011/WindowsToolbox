#pragma once

#include <string>
#include <Windows.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.UI.Xaml.h>

using namespace std;
using namespace winrt::Windows::Graphics;
using namespace winrt::Windows::UI::Xaml;

class MileWindow
{
public:
	MileWindow();

	PointInt32 Position;
	PointInt32 Size;
	PointInt32 MinWindowSize;
	PointInt32 MaxWindowSize;

	bool IsWindowCreated();

	string Title();
	void Title(string value);

	HWND Handle();

	UIElement Content();
	void Content(UIElement value);

	static MileWindow* Current();

	void InitializeWindow(HINSTANCE hInstance);
	void Activate(int nShowCmd);

private:
	bool _isWindowCreated;
	string _title;
	HWND _handle;
	UIElement _content = nullptr;
	static MileWindow* _current;

	void IsWindowCreated(bool value);
	void Handle(HWND value);
	void SetAppIcon();
	HICON LoadLocalExeIcon(LPCWSTR exeFile);
};