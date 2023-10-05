#pragma once

// https://github.com/microsoft/terminal
// /blob/c727762602b8bd12e4a3a769053204d7e92b81c5
// /src/cascadia/WindowsTerminalUniversal/pch.h#L12
// This is inexplicable, but for whatever reason, cppwinrt conflicts with the
// SDK definition of this function, so the only fix is to undef it.
// from WinBase.h
// Microsoft::UI::Xaml::Media::Animation::IStoryboard::GetCurrentTime
#ifdef GetCurrentTime
#undef GetCurrentTime
#endif

#include <Unknwn.h>
#include <Windows.h>
#include <hstring.h>
#include <winstring.h>
#include <winrt/base.h>
#include <winrt/Microsoft.UI.h>
#include <winrt/Microsoft.UI.Interop.h>
#include <winrt/Microsoft.UI.Windowing.h>
#include <winrt/Microsoft.UI.Xaml.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.h>