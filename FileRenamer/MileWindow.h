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
	MileWindow(string title = "", UIElement content = nullptr, PointInt32 position = { 0,0 }, PointInt32 size = { 0,0 });

	PointInt32 Position;
	PointInt32 Size;

	bool IsWindowCreated();

	string Title();
	void Title(string value);

	HWND Handle();

	UIElement Content();
	void Content(UIElement value);

	void InitializeWindow(HINSTANCE hInstance);
	void Activate(int nShowCmd);

private:
	void IsWindowCreated(bool value);
	void Handle(HWND value);

	bool _isWindowCreated;
	string _title;
	HWND _handle;
	UIElement _content = nullptr;
};