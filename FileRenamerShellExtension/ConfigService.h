#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.Storage.h>

/// <summary>
/// 设置选项配置服务
/// </summary>
class ConfigService
{
public:
	static winrt::Windows::Foundation::IInspectable ReadSettings(winrt::hstring key);
};
