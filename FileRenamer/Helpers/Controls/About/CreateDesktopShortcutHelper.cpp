#include "CreateDesktopShortcutHelper.h"

void CreateDesktopShortcutHelper::CreateDesktopShortcut()
{
	bool IsCreatedSuccessfully = false;
	HRESULT hr = CoInitialize(NULL);
	if (SUCCEEDED(hr))
	{
		IShellLink* AppLink = nullptr;
		hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, (void**)&AppLink);
		if (SUCCEEDED(hr))
		{
			//		Windows::Foundation::Collections::IVectorView<Core::AppListEntry> AppEntries = Package::Current().GetAppListEntriesAsync().GetResults();
			//		Core::AppListEntry DefaultEntry = AppEntries.GetAt(0);
			std::wstring AppLinkPath = std::wstring(L"shell:AppsFolder\\") + std::wstring(L"DefaultEntry.AppUserModelId().c_str()");
			AppLink->SetPath(AppLinkPath.c_str());

			IPersistFile* PersistFile = nullptr;
			hr = AppLink->QueryInterface(IID_IPersistFile, (void**)&PersistFile);
			if (SUCCEEDED(hr))
			{
				hr = PersistFile->Save(L"C:\\Users\\Gaoyifei\\Desktop\\01.lnk", false);
				if (SUCCEEDED(hr))
				{
					IsCreatedSuccessfully = true;
				}
				PersistFile->Release();
			}
			AppLink->Release();
		}
	}
	//CoUninitialize();
}