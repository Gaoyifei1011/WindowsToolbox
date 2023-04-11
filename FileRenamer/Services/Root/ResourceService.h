#pragma once

#include <winrt/Windows.ApplicationModel.Resources.Core.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "Models/Settings/Appearence/LanguageModel.h"
#include "LanguageModel.g.h"

namespace winrt
{
	namespace WinrtResourcesCore = Windows::ApplicationModel::Resources::Core;
}

/// <summary>
/// 应用资源服务
/// </summary>
class ResourceService
{
public:
	ResourceService();

	void InitializeResource(winrt::FileRenamer::LanguageModel defaultAppLanguage, winrt::FileRenamer::LanguageModel currentAppLanguage);
	winrt::hstring GetLocalized(winrt::hstring resource);

private:
	bool _isInitialized;
	winrt::FileRenamer::LanguageModel _defaultAppLanguage;
	winrt::FileRenamer::LanguageModel _currentAppLanguage;
	winrt::WinrtResourcesCore::ResourceContext _defaultResourceContext;
	winrt::WinrtResourcesCore::ResourceContext _currentResourceContext;
	winrt::WinrtResourcesCore::ResourceMap _resourceMap = (winrt::WinrtResourcesCore::ResourceManager::Current()).MainResourceMap();

	bool IsInitialized();
	void IsInitialized(bool value);

	winrt::FileRenamer::LanguageModel DefaultAppLanguage();
	void DefaultAppLanguage(winrt::FileRenamer::LanguageModel value);

	winrt::FileRenamer::LanguageModel CurrentAppLanguage();
	void CurrentAppLanguage(winrt::FileRenamer::LanguageModel value);

	winrt::WinrtResourcesCore::ResourceContext DefaultResourceContext();
	void DefaultResourceContext(winrt::WinrtResourcesCore::ResourceContext value);

	winrt::WinrtResourcesCore::ResourceContext CurrentResourceContext();
	void CurrentResourceContext(winrt::WinrtResourcesCore::ResourceContext value);

	winrt::WinrtResourcesCore::ResourceMap ResourceMap();
};
