#pragma once

#include <winrt/Windows.ApplicationModel.Resources.Core.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "winrt/impl/FileRenamer.2.h"
#include "Models/Settings/Appearence/LanguageModel.h"
#include "LanguageModel.g.h"
#include "ResourceService.h"

using namespace winrt;
using namespace winrt::FileRenamer;
using namespace winrt::Windows::ApplicationModel::Resources::Core;

bool ResourceService::_isInitialized = false;
ResourceMap ResourceService::_resourceMap = (ResourceManager::Current()).MainResourceMap();

bool ResourceService::IsInitialized()
{
	return _isInitialized;
}
void ResourceService::IsInitialized(bool value)
{
	_isInitialized = value;
}
//
//LanguageModel ResourceService::DefaultAppLanguage()
//{
//	return _defaultAppLanguage;
//}
//void ResourceService::DefualtAppLanguage(LanguageModel value)
//{
//	_defaultAppLanguage = value;
//}
//
//LanguageModel ResourceService::CurrentAppLanguage()
//{
//	return _currentAppLanguage;
//}
//void ResourceService::CurrentAppLanguage(LanguageModel value)
//{
//	_currentAppLanguage = value;
//}
//
//ResourceContext ResourceService::DefaultResourceContext()
//{
//	return _defaultResourceContext;
//}
//void ResourceService::DefaultResourceContext(ResourceContext value)
//{
//	_defaultResourceContext = value;
//}
//
//ResourceContext ResourceService::CurrentResourceContext()
//{
//	return _currentResourceContext;
//}
//void ResourceService::CurrentResourceContext(ResourceContext value)
//{
//	_currentResourceContext = value;
//}

ResourceMap ResourceService::ResourceMap()
{
	return _resourceMap;
}

/// <summary>
/// 初始化应用本地化资源
/// </summary>
/// <param name="defaultAppLanguage">默认语言名称</param>
/// <param name="currentAppLanguage">当前语言名称</param>
void ResourceService::InitializeResource(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage)
{
	//ResourceService::DefualtAppLanguage(defaultAppLanguage);
	//ResourceService::CurrentAppLanguage(currentAppLanguage);

	//ResourceService::DefaultResourceContext().QualifierValues().Insert(L"Language", ResourceService::DefaultAppLanguage().InternalName());
	//ResourceService::CurrentResourceContext().QualifierValues().Insert(L"Language", ResourceService::CurrentAppLanguage().InternalName());

	IsInitialized(true);
}

/// <summary>
/// 字符串本地化
/// </summary>
hstring ResourceService::GetLocalized(hstring resource)
{
	if (ResourceService::IsInitialized())
	{
		//try
		//{
		//	return ResourceService::ResourceMap().GetValue(resource, CurrentResourceContext()).ValueAsString();
		//}
		//catch (const std::exception&)
		//{
		//	try
		//	{
		//		return ResourceService::ResourceMap().GetValue(resource, DefaultResourceContext()).ValueAsString();
		//	}
		//	catch (const std::exception&)
		//	{
		//		return resource;
		//	}
		//}
	}
	else
	{
		throw "ResourcesInitializeFailed";
	}
}