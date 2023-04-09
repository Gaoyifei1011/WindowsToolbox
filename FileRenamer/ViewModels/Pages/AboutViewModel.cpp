#include "pch.h"
#include "AboutViewModel.h"
#include "AboutViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AboutViewModel::AboutViewModel()
	{
		_createDesktopShortcutCommand = winrt::make<RelayCommand>([this](IInspectable parameter)
			{
				//CreateDesktopShortcutHelper::CreateDesktopShortcut();
				//bool IsCreatedSuccessfully = false;
				//HRESULT hr = CoInitialize(NULL);
				//if (SUCCEEDED(hr))
				//{
				//	IShellLink* AppLink = nullptr;
				//	hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, (void**)&AppLink);
				//	if (SUCCEEDED(hr))
				//	{
				//		Windows::Foundation::Collections::IVectorView<Core::AppListEntry> AppEntries = Package::Current().GetAppListEntriesAsync().GetResults();
				//		Core::AppListEntry DefaultEntry = AppEntries.GetAt(0);
				//		wstring AppLinkPath = wstring(L"shell:AppsFolder\\") + wstring(DefaultEntry.AppUserModelId().c_str());
				//		AppLink->SetPath(AppLinkPath.c_str());

				//		IPersistFile* PersistFile = nullptr;
				//		hr = AppLink->QueryInterface(IID_IPersistFile, (void**)&PersistFile);
				//		if (SUCCEEDED(hr))
				//		{
				//			hr = PersistFile->Save(L"C:\\Users\\Gaoyifei\\Desktop\\01.lnk", false);
				//			if (SUCCEEDED(hr))
				//			{
				//				IsCreatedSuccessfully = true;
				//			}
				//			PersistFile->Release();
				//		}
				//		AppLink->Release();
				//	}
				//}
				//CoUninitialize();
			});

		_pinToStartScreenCommand = winrt::make<RelayCommand>([this](IInspectable parameter) -> IAsyncAction
			{
				bool IsPinnedSuccessfully = false;

				Collections::IVectorView<winrt::Windows::ApplicationModel::Core::AppListEntry> AppEntries = co_await winrt::Windows::ApplicationModel::Package::Current().GetAppListEntriesAsync();
				winrt::Windows::ApplicationModel::Core::AppListEntry DefaultEntry = AppEntries.GetAt(0);

				if (DefaultEntry != nullptr)
				{
					winrt::Windows::UI::StartScreen::StartScreenManager startScreenManager = winrt::Windows::UI::StartScreen::StartScreenManager::GetDefault();
					bool containsEntry = co_await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

					if (!containsEntry)
					{
						startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
					}
				}
				IsPinnedSuccessfully = true;
			});

		_pinToTaskbarCommand = make<RelayCommand>([this](IInspectable parameter)
			{
			});

		_showReleaseNotesCommand = make<RelayCommand>([this](IInspectable parameter) -> IAsyncAction
			{
				Uri uri(L"https://github.com/Gaoyifei1011/FileRenamer/releases");
				co_await winrt::Windows::System::Launcher::LaunchUriAsync(uri);
			});

		_showLicenseCommand = make<RelayCommand>([this](IInspectable parameter)
			{
			});
	};

	/// <summary>
	/// 创建应用的桌面快捷方式
	/// </summary>
	ICommand AboutViewModel::CreateDesktopShortcutCommand()
	{
		return _createDesktopShortcutCommand;
	}

	/// <summary>
	/// 将应用固定到“开始”屏幕
	/// </summary>
	ICommand AboutViewModel::PinToStartScreenCommand()
	{
		return _pinToStartScreenCommand;
	}

	/// <summary>
	/// 将应用固定到任务栏
	/// </summary>
	ICommand AboutViewModel::PinToTaskbarCommand()
	{
		return _pinToTaskbarCommand;
	}

	/// <summary>
	/// 查看更新日志
	/// </summary>
	ICommand AboutViewModel::ShowReleaseNotesCommand()
	{
		return _showReleaseNotesCommand;
	}

	/// <summary>
	/// 查看许可证
	/// </summary>
	ICommand AboutViewModel::ShowLicenseCommand()
	{
		return _showLicenseCommand;
	}
}