#include "pch.h"
#include "BaseExplorerCommand.h"
#include <mutex>

// 导出的 GUID 值
struct DECLSPEC_UUID("5A730150-DE8D-0C88-FD1A-99B7E954BDDB") ShellExtensionClassFactory : winrt::implements<ShellExtensionClassFactory, IClassFactory>
{
	HRESULT __stdcall CreateInstance(IUnknown * outer, GUID const& iid, void** result) noexcept final
	{
		*result = nullptr;

		if (outer)
		{
			return CLASS_E_NOAGGREGATION;
		}

		InitializeResource();

		winrt::com_ptr<winrt::FileRenamerShellExtension::implementation::FileRenamerCommand> fileRenamerCommand = winrt::make_self<winrt::FileRenamerShellExtension::implementation::FileRenamerCommand>();
		winrt::hresult convertResult = fileRenamerCommand.as(winrt::guid_of<IExplorerCommand>(), result);
		return S_OK;
	}

	HRESULT __stdcall LockServer(BOOL) noexcept final
	{
		return S_OK;
	}
};

bool __stdcall winrt_can_unload_now() noexcept
{
	if (winrt::get_module_lock())
	{
		return false;
	}

	winrt::clear_factory_cache();
	return true;
}

EXTERN_C HRESULT STDAPICALLTYPE DllCanUnloadNow()
{
	return winrt_can_unload_now() ? S_OK : S_FALSE;
}

EXTERN_C HRESULT STDAPICALLTYPE DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID* ppv)
{
	if (!ppv)
	{
		return E_POINTER;
	}

	if (riid != IID_IClassFactory && riid != IID_IUnknown)
	{
		return E_NOINTERFACE;
	}

	if (rclsid != __uuidof(ShellExtensionClassFactory))
	{
		return E_INVALIDARG;
	}

	try
	{
		return winrt::make<ShellExtensionClassFactory>()->QueryInterface(riid, ppv);
	}
	catch (...)
	{
		return winrt::to_hresult();
	}
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}