#include "pch.h"
#include "AboutViewModel.h"
#include "AboutViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AboutViewModel::AboutViewModel()
	{
		_createDesktopShortcutCommand = winrt::make<FileRenamer::implementation::RelayCommand>([this](WinrtFoundation::IInspectable parameter) -> WinrtFoundation::IAsyncAction
			{
				bool IsCreatedSuccessfully = false;
				HRESULT hr = CoInitialize(NULL);
				if (SUCCEEDED(hr))
				{
					IShellLink* AppLink = nullptr;
					hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, (void**)&AppLink);
					if (SUCCEEDED(hr))
					{
						WinrtCollections::IVectorView<WinrtApplicationModelCore::AppListEntry> AppEntries = co_await WinrtApplicationModel::Package::Current().GetAppListEntriesAsync();
						WinrtApplicationModelCore::AppListEntry DefaultEntry = AppEntries.GetAt(0);
						std::wstring AppLinkPath = std::wstring(L"shell:AppsFolder\\") + std::wstring(DefaultEntry.AppUserModelId().c_str());
						AppLink->SetPath(AppLinkPath.c_str());

						IPersistFile* PersistFile = nullptr;
						hr = AppLink->QueryInterface(IID_IPersistFile, (void**)&PersistFile);
						if (SUCCEEDED(hr))
						{
							TCHAR szDesktop[MAX_PATH];
							LPITEMIDLIST pidl = NULL;
							SHGetFolderLocation(NULL, CSIDL_DESKTOPDIRECTORY, NULL, 0, &pidl);
							SHGetPathFromIDList(pidl, szDesktop);

							TCHAR szLinkPath[MAX_PATH];
							ZeroMemory(szLinkPath, MAX_PATH);

							wsprintf(szLinkPath, L"%s\\%s.lnk", szDesktop, AppResourcesService.GetLocalized(L"Resources/AppDisplayName").c_str());
							hr = PersistFile->Save(szLinkPath, false);
							if (SUCCEEDED(hr))
							{
								IsCreatedSuccessfully = true;
							}
							PersistFile->Release();
						}
						AppLink->Release();
					}
				}
				CoUninitialize();
			});

		_pinToStartScreenCommand = winrt::make<FileRenamer::implementation::RelayCommand>([this](WinrtFoundation::IInspectable parameter) -> WinrtFoundation::IAsyncAction
			{
				bool IsPinnedSuccessfully = false;

				WinrtCollections::IVectorView<WinrtApplicationModelCore::AppListEntry> AppEntries = co_await WinrtApplicationModel::Package::Current().GetAppListEntriesAsync();
				WinrtApplicationModelCore::AppListEntry DefaultEntry = AppEntries.GetAt(0);

				if (DefaultEntry != nullptr)
				{
					WinrtStartScreen::StartScreenManager startScreenManager = WinrtStartScreen::StartScreenManager::GetDefault();
					bool containsEntry = co_await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

					if (!containsEntry)
					{
						startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
					}
				}
				IsPinnedSuccessfully = true;
			});

		_pinToTaskbarCommand = make<FileRenamer::implementation::RelayCommand>([this](WinrtFoundation::IInspectable parameter)
			{
			});

		_showReleaseNotesCommand = make<FileRenamer::implementation::RelayCommand>([this](WinrtFoundation::IInspectable parameter) -> WinrtFoundation::IAsyncAction
			{
				WinrtFoundation::Uri uri(L"https://github.com/Gaoyifei1011/FileRenamer/releases");
				co_await WinrtSystem::Launcher::LaunchUriAsync(uri);
			});

		_showLicenseCommand = make<FileRenamer::implementation::RelayCommand>([this](WinrtFoundation::IInspectable parameter)
			{
			});
	};

	/// <summary>
	/// 创建应用的桌面快捷方式
	/// </summary>
	WinrtInput::ICommand AboutViewModel::CreateDesktopShortcutCommand()
	{
		return _createDesktopShortcutCommand;
	}

	/// <summary>
	/// 将应用固定到“开始”屏幕
	/// </summary>
	WinrtInput::ICommand AboutViewModel::PinToStartScreenCommand()
	{
		return _pinToStartScreenCommand;
	}

	/// <summary>
	/// 将应用固定到任务栏
	/// </summary>
	WinrtInput::ICommand AboutViewModel::PinToTaskbarCommand()
	{
		return _pinToTaskbarCommand;
	}

	/// <summary>
	/// 查看更新日志
	/// </summary>
	WinrtInput::ICommand AboutViewModel::ShowReleaseNotesCommand()
	{
		return _showReleaseNotesCommand;
	}

	/// <summary>
	/// 查看许可证
	/// </summary>
	WinrtInput::ICommand AboutViewModel::ShowLicenseCommand()
	{
		return _showLicenseCommand;
	}
}