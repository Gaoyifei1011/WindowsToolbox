#pragma once

#include <string>
#include <Windows.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.Graphics.h>
#include <winrt/Windows.Storage.h>
#include <winrt/Windows.UI.Core.h>
#include <winrt/Windows.UI.Xaml.h>
#include <winrt/Windows.UI.Xaml.Media.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Controls.Primitives.h>

#include "Helpers/Root/DPICalcHelper.h"

namespace winrt
{
	namespace WinrtApplicationModel = Windows::ApplicationModel;
	namespace WinrtCore = Windows::UI::Core;
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtGraphics = Windows::Graphics;
	namespace WinrtMedia = Windows::UI::Xaml::Media;
	namespace WinrtPrimitives = Windows::UI::Xaml::Controls::Primitives;
	namespace WinrtStorage = Windows::Storage;
	namespace WinrtXaml = Windows::UI::Xaml;
}

class MileWindow
{
public:
	MileWindow();

	winrt::WinrtGraphics::PointInt32 Position;
	winrt::WinrtGraphics::PointInt32 Size;
	winrt::WinrtGraphics::PointInt32 MinWindowSize;
	winrt::WinrtGraphics::PointInt32 MaxWindowSize;

	bool IsWindowCreated();

	std::string Title();
	void Title(std::string value);

	HWND Handle();

	winrt::WinrtXaml::UIElement Content();
	void Content(winrt::WinrtXaml::UIElement value);

	static MileWindow* Current();

	void InitializeWindow(HINSTANCE hInstance);
	void Activate(int nShowCmd);

private:
	bool _isWindowCreated;
	std::string _title;
	HWND _handle;
	winrt::WinrtXaml::UIElement _content = nullptr;
	static MileWindow* _current;

	void IsWindowCreated(bool value);
	void Handle(HWND value);
	void SetAppIcon();
	HICON LoadLocalExeIcon(LPCWSTR exeFile);
};