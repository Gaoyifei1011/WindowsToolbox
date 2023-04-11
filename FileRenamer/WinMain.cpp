#include <Windows.h>
#include <WinMain.h>
#include "pch.h"
#include <tchar.h>
#include "App.h"
#include "MainPage.h"
#include "Services/Root/ResourceService.h"
#include "Services/Window/NavigationService.h"

ResourceService AppResourcesService;
NavigationService AppNavigationService;

void ApplicationStart(HINSTANCE hInstance, int nShowCmd);
void InitializeProgramResources();
bool IsAppLaunched(LPCWSTR pszUniqueName);

/// <summary>
/// 文件重命名工具
/// </summary>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
	UNREFERENCED_PARAMETER(hPrevInstance);

	InitializeProgramResources();

	// 检测应用是否处于运行状态
	if (IsAppLaunched(L"Gaoyifei1011.FileRenamer") == true)
	{
		// 如果应用程序的启动参数是重启，则直接启动第二个实例，并关闭之前的实例
		if (_tcscmp(lpCmdLine, L"Restart") == 0)
		{
			ApplicationStart(hInstance, nShowCmd);
		}
		// 关闭当前实例，并重定向到第一个实例
		else
		{
			// 重定向操作
			return 0;
		}
	}
	// 没有，运行第一个实例
	else
	{
		ApplicationStart(hInstance, nShowCmd);
	}

	return 0;
}

/// <summary>
/// 初始化并启动应用实例
/// </summary>
void ApplicationStart(HINSTANCE hInstance, int nShowCmd)
{
	winrt::init_apartment(winrt::apartment_type::single_threaded);
	winrt::com_ptr<winrt::FileRenamer::implementation::App> ApplicationRoot = winrt::make_self<winrt::FileRenamer::implementation::App>();
	ApplicationRoot->Run(hInstance, nShowCmd);
}

/// <summary>
/// 加载应用程序所需的资源
/// </summary>
void InitializeProgramResources()
{
	winrt::FileRenamer::LanguageModel defaultLanguage = winrt::make<winrt::FileRenamer::implementation::LanguageModel>();
	winrt::FileRenamer::LanguageModel currentLanguage = winrt::make<winrt::FileRenamer::implementation::LanguageModel>();

	defaultLanguage.DisplayName(L"English (United States)");
	currentLanguage.DisplayName(L"中文（简体）");
	defaultLanguage.InternalName(L"en-us");
	currentLanguage.InternalName(L"zh-hans");
	AppResourcesService.InitializeResource(defaultLanguage, currentLanguage);
}

/// <summary>
/// 检测应用程序是否已经运行
/// </summary>
bool IsAppLaunched(LPCWSTR pszUniqueName)
{
	HANDLE hMutex = CreateEvent(NULL, TRUE, FALSE, pszUniqueName);
	DWORD dwLstErr = GetLastError();
	bool isAppLaunched = false;

	if (hMutex)
	{
		if (dwLstErr == ERROR_ALREADY_EXISTS)
		{
			CloseHandle(hMutex);
			isAppLaunched = true;
		}
	}
	else
	{
		if (dwLstErr == ERROR_ACCESS_DENIED)
		{
			isAppLaunched = true;
		}
	}

	return isAppLaunched;
}