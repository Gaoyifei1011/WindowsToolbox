#pragma once

#include "pch.h"
#include <filesystem>
#include <shobjidl_core.h>
#include <shlwapi.h>
#include <sstream>
#include "BaseExplorerCommand.g.h"
#include "FileRenamerCommand.g.h"
#include "FileNameCommand.g.h"
#include "ExtensionNameCommand.g.h"
#include "FilePropertiesCommand.g.h"
#include "UpperAndLowerCaseCommand.g.h"

void InitializeResource();
winrt::hstring GetLocalized(winrt::hstring resource);
winrt::Windows::Foundation::IInspectable ReadSettings(winrt::hstring key);

namespace winrt::FileRenamerShellExtension::implementation
{
	struct SubMenu : winrt::implements<SubMenu, IEnumExplorerCommand>
	{
	public:
		SubMenu(winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> commands) : mCommands(std::move(commands)) {};
		IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched);
		IFACEMETHODIMP Skip(ULONG /*celt*/);
		IFACEMETHODIMP Reset();
		IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum);

	private:
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> mCommands;
		uint32_t mIndex{};
	};

	struct BaseExplorerCommand : BaseExplorerCommandT<BaseExplorerCommand, IExplorerCommand>
	{
	public:
		BaseExplorerCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip);
		IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags);
		IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands);
		virtual winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};

	struct FileRenamerCommand : FileRenamerCommandT<FileRenamerCommand, BaseExplorerCommand>
	{
	public:
		FileRenamerCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};

	struct FileNameCommand : FileNameCommandT<FileNameCommand, BaseExplorerCommand>
	{
	public:
		FileNameCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};

	struct ExtensionNameCommand : ExtensionNameCommandT<ExtensionNameCommand, BaseExplorerCommand>
	{
	public:
		ExtensionNameCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};

	struct UpperAndLowerCaseCommand : UpperAndLowerCaseCommandT<UpperAndLowerCaseCommand, BaseExplorerCommand>
	{
	public:
		UpperAndLowerCaseCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};

	struct FilePropertiesCommand : FilePropertiesCommandT<FilePropertiesCommand, BaseExplorerCommand>
	{
	public:
		FilePropertiesCommand();
		IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);
		IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
		IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);
		IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*);
		winrt::Windows::Foundation::Collections::IVectorView<winrt::FileRenamerShellExtension::BaseExplorerCommand> SubCommands();
	};
}

namespace winrt::FileRenamerShellExtension::factory_implementation
{
	struct BaseExplorerCommand : BaseExplorerCommandT<BaseExplorerCommand, winrt::FileRenamerShellExtension::implementation::BaseExplorerCommand>
	{
	};
}
