#include "pch.h"
#include "ConfigService.h"

/// <summary>
/// 读取设置选项存储信息
/// </summary>
winrt::Windows::Foundation::IInspectable ConfigService::ReadSettings(winrt::hstring key)
{
	if (winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().TryLookup(key) == nullptr)
	{
		return nullptr;
	}

	return winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().Lookup(key);
}