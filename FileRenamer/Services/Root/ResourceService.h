#pragma once

#include <winrt/Windows.ApplicationModel.Resources.Core.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "winrt/impl/FileRenamer.2.h"
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
	static void InitializeResource(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage);
	static hstring GetLocalized(hstring resource);

private:
	static bool _isInitialized;
	//static LanguageModel _defaultAppLanguage;
	//static LanguageModel _currentAppLanguage;
	//static ResourceContext _defaultResourceContext;
	//static ResourceContext _currentResourceContext;
	static ResourceMap _resourceMap;

	static bool IsInitialized();
	static void IsInitialized(bool value);

	//static LanguageModel DefaultAppLanguage();
	//static void DefualtAppLanguage(LanguageModel value);

	//static LanguageModel CurrentAppLanguage();
	//static void CurrentAppLanguage(LanguageModel value);

	//static ResourceContext DefaultResourceContext();
	//static void DefaultResourceContext(ResourceContext value);

	//static ResourceContext CurrentResourceContext();
	//static void CurrentResourceContext(ResourceContext value);

	static ResourceMap ResourceMap();
};

