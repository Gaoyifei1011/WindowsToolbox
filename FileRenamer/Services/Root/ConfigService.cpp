#include "ConfigService.h"

ConfigService::ConfigService() {}

/// <summary>
/// 读取设置选项存储信息
/// </summary>
winrt::WinrtFoundation::IInspectable ConfigService::ReadSettings(winrt::hstring key)
{
	if (winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().TryLookup(key) == nullptr)
	{
		return nullptr;
	}

	return winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().Lookup(key);
}

/// <summary>
/// 保存设置选项存储信息
/// </summary>
void ConfigService::SaveSettings(winrt::hstring key, winrt::WinrtFoundation::IInspectable value)
{
	winrt::WinrtStorage::ApplicationData::Current().LocalSettings().Values().Insert(key, value);
}