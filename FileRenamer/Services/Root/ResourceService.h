#pragma once

#include <winrt/Windows.ApplicationModel.Resources.Core.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "Models/Settings/Appearence/LanguageModel.h"
#include "LanguageModel.g.h"

using namespace winrt;
using namespace winrt::FileRenamer;
using namespace winrt::Windows::ApplicationModel::Resources::Core;

/// <summary>
/// 应用资源服务
/// </summary>
class ResourceService
{
public:
	ResourceService();

	void InitializeResource(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage);
	hstring GetLocalized(hstring resource);

private:
	bool _isInitialized;
	LanguageModel _defaultAppLanguage;
	LanguageModel _currentAppLanguage;
	ResourceContext _defaultResourceContext;
	ResourceContext _currentResourceContext;
	ResourceMap _resourceMap = (ResourceManager::Current()).MainResourceMap();

	bool IsInitialized();
	void IsInitialized(bool value);

	LanguageModel DefaultAppLanguage();
	void DefaultAppLanguage(LanguageModel value);

	LanguageModel CurrentAppLanguage();
	void CurrentAppLanguage(LanguageModel value);

	ResourceContext DefaultResourceContext();
	void DefaultResourceContext(ResourceContext value);

	ResourceContext CurrentResourceContext();
	void CurrentResourceContext(ResourceContext value);

	ResourceMap ResourceMap();
};

