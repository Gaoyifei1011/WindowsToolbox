#include "LanguageModel.g.h"
#include "ResourceService.h"

ResourceService::ResourceService()
{
	_isInitialized = false;
}

bool ResourceService::IsInitialized()
{
	return _isInitialized;
}
void ResourceService::IsInitialized(bool value)
{
	_isInitialized = value;
}

winrt::FileRenamer::LanguageModel ResourceService::DefaultAppLanguage()
{
	return _defaultAppLanguage;
}
void ResourceService::DefaultAppLanguage(winrt::FileRenamer::LanguageModel value)
{
	_defaultAppLanguage = value;
}

winrt::FileRenamer::LanguageModel ResourceService::CurrentAppLanguage()
{
	return _currentAppLanguage;
}
void ResourceService::CurrentAppLanguage(winrt::FileRenamer::LanguageModel value)
{
	_currentAppLanguage = value;
}

winrt::WinrtResourcesCore::ResourceContext ResourceService::DefaultResourceContext()
{
	return _defaultResourceContext;
}
void ResourceService::DefaultResourceContext(winrt::WinrtResourcesCore::ResourceContext value)
{
	_defaultResourceContext = value;
}

winrt::WinrtResourcesCore::ResourceContext ResourceService::CurrentResourceContext()
{
	return _currentResourceContext;
}
void ResourceService::CurrentResourceContext(winrt::WinrtResourcesCore::ResourceContext value)
{
	_currentResourceContext = value;
}

winrt::WinrtResourcesCore::ResourceMap ResourceService::ResourceMap()
{
	return _resourceMap;
}

/// <summary>
/// 初始化应用本地化资源
/// </summary>
/// <param name="defaultAppLanguage">默认语言名称</param>
/// <param name="currentAppLanguage">当前语言名称</param>
void ResourceService::InitializeResource(winrt::FileRenamer::LanguageModel defaultAppLanguage, winrt::FileRenamer::LanguageModel currentAppLanguage)
{
	ResourceService::DefaultAppLanguage(defaultAppLanguage);
	ResourceService::CurrentAppLanguage(currentAppLanguage);
	ResourceService::DefaultResourceContext().QualifierValues().Insert(L"Language", ResourceService::DefaultAppLanguage().InternalName());
	ResourceService::CurrentResourceContext().QualifierValues().Insert(L"Language", ResourceService::CurrentAppLanguage().InternalName());

	IsInitialized(true);
}

/// <summary>
/// 字符串本地化
/// </summary>
winrt::hstring ResourceService::GetLocalized(winrt::hstring resource)
{
	if (ResourceService::IsInitialized())
	{
		try
		{
			return ResourceService::ResourceMap().GetValue(resource, ResourceService::CurrentResourceContext()).ValueAsString();
		}
		catch (const std::exception&)
		{
			try
			{
				return ResourceService::ResourceMap().GetValue(resource, ResourceService::DefaultResourceContext()).ValueAsString();
			}
			catch (const std::exception&)
			{
				return resource;
			}
		}
	}
	else
	{
		throw "ResourcesInitializeFailed";
	}
}