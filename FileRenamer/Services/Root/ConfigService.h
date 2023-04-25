#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.Storage.h>

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtStorage = Windows::Storage;
}

/// <summary>
/// 设置选项配置服务
/// </summary>
class ConfigService
{
public:
	ConfigService();

	winrt::WinrtFoundation::IInspectable ReadSettings(winrt::hstring key);
	void SaveSettings(winrt::hstring key, winrt::WinrtFoundation::IInspectable value);
};
