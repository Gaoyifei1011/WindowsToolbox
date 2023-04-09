#include "pch.h"
#include "ReferenceViewModel.h"
#include "ReferenceViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::Foundation;

namespace winrt::FileRenamer::implementation
{
	ReferenceViewModel::ReferenceViewModel()
	{
		FileRenamer::ReferenceKeyValuePairModel ReferenceItem = make<ReferenceKeyValuePairModel>();
		ReferenceItem.Key(L"Microsoft.Windows.CppWinRT");
		ReferenceItem.Value(L"https://github.com/Microsoft/cppwinrt");
		_referenceDict.Append(ReferenceItem);
		//_referenceDict.Append(make<ReferenceKeyValuePairModel>(L"Microsoft.Windows.CppWinRT", L"https://github.com/Microsoft/cppwinrt"));
		//_referenceDict.Append(make<ReferenceKeyValuePairModel>(L"Microsoft.Windows.SDK.BuildTools", L"https://aka.ms/WinSDKProjectURL"));
		//_referenceDict.Append(make<ReferenceKeyValuePairModel>(L"Mile.Xaml", L"https://github.com/ProjectMile/Mile.Xaml"));
		auto item = _referenceDict;
		auto length = item.Size();
	}

	Collections::IVector<FileRenamer::ReferenceKeyValuePairModel> ReferenceViewModel::ReferenceDict()
	{
		return _referenceDict;
	}
}