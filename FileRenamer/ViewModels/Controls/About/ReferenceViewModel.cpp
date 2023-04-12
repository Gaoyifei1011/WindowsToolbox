#include "pch.h"
#include "ReferenceViewModel.h"
#include "ReferenceViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ReferenceViewModel::ReferenceViewModel()
	{
		_referenceDict.Append(winrt::make<ReferenceKeyValuePairModel>(L"Microsoft.Windows.CppWinRT", L"https://github.com/Microsoft/cppwinrt"));
		_referenceDict.Append(winrt::make<ReferenceKeyValuePairModel>(L"Microsoft.Windows.SDK.BuildTools", L"https://aka.ms/WinSDKProjectURL"));
		_referenceDict.Append(winrt::make<ReferenceKeyValuePairModel>(L"Mile.Xaml", L"https://github.com/ProjectMile/Mile.Xaml"));
	}

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ReferenceKeyValuePairModel> ReferenceViewModel::ReferenceDict()
	{
		return _referenceDict;
	}
}