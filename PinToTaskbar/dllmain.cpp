// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"

#ifdef PINTOTASKBAR_EXPORTS
#define HOOK_API __declspec(dllexport)
#else
#define HOOK_API __declspec(dllimport)
#endif

#ifndef IID_PPV_ARG
#define IID_PPV_ARG(IType, ppType) IID_##IType, reinterpret_cast<void**>(static_cast<IType**>(ppType))
#endif

#include <windows.h>
#include <tchar.h>

#include <commctrl.h>
#pragma comment (lib, "Comctl32")

#include <ShObjIdl_core.h>
#include <Shlobj.h>
#include <shlwapi.h>
#pragma comment (lib, "Shlwapi")

#pragma data_seg("Shared")
HHOOK hookInstance = NULL;
#pragma data_seg()
#pragma comment(linker, "/section:Shared,rws")

HINSTANCE hModuleInstance = NULL;

#define WM_PINMESSAGE WM_USER + 10
#define WM_PINNOTIFY WM_USER + 11

LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam);
int CompareMenuStrings(LPCWSTR s1, LPCWSTR s2);
HRESULT PinToTaskbar(LPCWSTR wsPathFile, int* nPinned);

/// <summary>
/// DLL 函数入口点
/// </summary>
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		hModuleInstance = (HINSTANCE)hModule;
		break;
	}
	case DLL_THREAD_ATTACH: break;
	case DLL_THREAD_DETACH: break;
	case DLL_PROCESS_DETACH: break;
	}
	return TRUE;
}

/// <summary>
/// 安装钩子
/// </summary>
HOOK_API void StartHook(DWORD threadId)
{
	if (hookInstance == NULL)
	{
		hookInstance = SetWindowsHookEx(WH_GETMESSAGE, (HOOKPROC)GetMsgProc, hModuleInstance, threadId);
	}
}

HOOK_API void StopHook()
{
	if (hookInstance != NULL)
	{
		UnhookWindowsHookEx(hookInstance);
		hookInstance = NULL;
	}
}

/// <summary>
/// 处理钩子消息
/// </summary>
LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	MSG* lpMsg;
	if ((nCode >= 0) && (PM_REMOVE == wParam))
	{
		lpMsg = (MSG*)lParam;
		if (lpMsg->message == WM_PINMESSAGE)
		{
			//Beep(9000, 10);
			HANDLE hMapFile = OpenFileMapping(FILE_MAP_READ, FALSE, TEXT("PinSharedMemory"));
			if (hMapFile)
			{
				LPVOID pSharedData = MapViewOfFile(hMapFile, FILE_MAP_READ, 0, 0, 0);
				if (pSharedData)
				{
					WCHAR wsPath[MAX_PATH] = { 0 };
					lstrcpy(wsPath, (WCHAR*)pSharedData);
					int nPinned = 0;
					HRESULT hr = PinToTaskbar(wsPath, &nPinned);
					if (lpMsg->lParam != NULL)
					{
						PostMessage((HWND)lpMsg->lParam, WM_PINNOTIFY, nPinned, hr);
					}
					UnmapViewOfFile(pSharedData);
				}
				CloseHandle(hMapFile);
			}
		}
	}

	LRESULT lResult = 0;
	lResult = CallNextHookEx(hookInstance, nCode, wParam, lParam);
	return lResult;
}

/// <summary>
/// 固定应用到任务栏
/// </summary>
HRESULT PinToTaskbar(LPCWSTR wsPathFile, int* nPinned)
{
	// https://github.com/fei018/OmegaT-Windows/blob/4a4df43a5dfaf78534d0eeb70258200428591030/Windows/source/8/mui/Windows/System32/be-BY/shell32.dll_6.2.8102.0_x32.rc#L2926
	WCHAR wszPinLocalizedVerb[128] = { 0 };

	HINSTANCE hShell32 = LoadLibrary(TEXT("Shell32.dll"));
	if (hShell32)
	{
		LoadString(hShell32, static_cast<UINT>(5386), wszPinLocalizedVerb, 128);
		FreeLibrary(hShell32);

		HRESULT hr = E_FAIL;
		LPITEMIDLIST pItemIDL = ILCreateFromPath(wsPathFile);

		if (pItemIDL)
		{
			LPCITEMIDLIST aPidl[512]{};
			LPITEMIDLIST pidlRel = NULL;
			UINT nIndex = 0;
			LPCITEMIDLIST pidlChild;
			IShellFolder* psf;

			pidlRel = ILFindLastID(pItemIDL);
			aPidl[nIndex++] = ILClone(pidlRel);
			hr = SHBindToParent(pItemIDL, IID_PPV_ARGS(&psf), &pidlChild);
			if (SUCCEEDED(hr))
			{
				IContextMenu* pContextMenu = nullptr;
				DEFCONTEXTMENU dcm = { NULL, NULL, NULL, psf, nIndex, (LPCITEMIDLIST*)aPidl, NULL, 0, NULL };
				hr = SHCreateDefaultContextMenu(&dcm, IID_IContextMenu, (void**)&pContextMenu);
				if (SUCCEEDED(hr))
				{
					HMENU hMenu = CreatePopupMenu();
					hr = pContextMenu->QueryContextMenu(hMenu, 0, 1, 0x7fff, CMF_NORMAL);
					if (SUCCEEDED(hr))
					{
						int nNbItems = GetMenuItemCount(hMenu);
						for (int i = nNbItems - 1; i >= 0; i--)
						{
							MENUITEMINFO mii = {};
							mii.cbSize = sizeof(mii);
							mii.fMask = MIIM_FTYPE | MIIM_STRING | MIIM_ID | MIIM_SUBMENU | MIIM_DATA | MIIM_STATE;
							mii.cch = 128;
							TCHAR pszName[128] = {};
							mii.dwTypeData = pszName;
							if (GetMenuItemInfo(hMenu, i, TRUE, &mii) && CompareMenuStrings(pszName, wszPinLocalizedVerb) == 0)
							{
								CMINVOKECOMMANDINFOEX invoke = {};
								invoke.cbSize = sizeof(invoke);
								invoke.fMask = 0x00004000;
								invoke.hwnd = GetDesktopWindow();
								invoke.lpVerb = MAKEINTRESOURCEA(mii.wID - 1);
								invoke.lpVerbW = MAKEINTRESOURCEW(mii.wID - 1);
								invoke.nShow = SW_SHOWNORMAL;
								hr = pContextMenu->InvokeCommand((LPCMINVOKECOMMANDINFO)&invoke);
								if (SUCCEEDED(hr))
								{
									if (CompareMenuStrings(pszName, wszPinLocalizedVerb) == 0)
										*nPinned = 1;
								}
								break;
							}
						}
					}
					DestroyMenu(hMenu);
					pContextMenu->Release();
				}
				psf->Release();
			}
			ILFree((LPITEMIDLIST)aPidl[0]);
			ILFree(pItemIDL);
		}
		return hr;
	}
	else
	{
		return 0;
	}
}

/// <summary>
/// 字符串比较
/// </summary>
int CompareMenuStrings(LPCWSTR s1, LPCWSTR s2)
{
	while (*s1 != L'\0' && *s2 != L'\0')
	{
		// Skip ampersands
		while (*s1 == L'&') ++s1;
		while (*s2 == L'&') ++s2;
		if (towlower(*s1) != towlower(*s2))
			return (int)(towlower(*s1) - towlower(*s2));
		++s1;
		++s2;
	}

	// Skip trailing ampersands
	while (*s1 == L'&') ++s1;
	while (*s2 == L'&') ++s2;
	return (int)(towlower(*s1) - towlower(*s2));
}