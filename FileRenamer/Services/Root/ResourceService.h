#pragma once

#include <winrt/Windows.ApplicationModel.Resources.Core.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "Models/Settings/Appearence/LanguageModel.h"
#include "Models/Settings/Appearence/ThemeModel.h"
#include "LanguageModel.g.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtResourcesCore = Windows::ApplicationModel::Resources::Core;
}

/// <summary>
/// 应用资源服务
/// </summary>
class ResourceService
{
public:
	ResourceService();

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> ThemeList();

	void InitializeResource(winrt::FileRenamer::LanguageModel defaultAppLanguage, winrt::FileRenamer::LanguageModel currentAppLanguage);
	void LocalizeResource();
	void InitializeThemeList();
	winrt::hstring GetLocalized(winrt::hstring resource);

private:
	bool _isInitialized{ false };
	winrt::FileRenamer::LanguageModel _defaultAppLanguage{ nullptr };
	winrt::FileRenamer::LanguageModel _currentAppLanguage{ nullptr };
	winrt::WinrtResourcesCore::ResourceContext _defaultResourceContext;
	winrt::WinrtResourcesCore::ResourceContext _currentResourceContext;
	winrt::WinrtResourcesCore::ResourceMap _resourceMap{ nullptr };

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

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> _themeList{ winrt::single_threaded_observable_vector<winrt::FileRenamer::ThemeModel>() };
};
