#include "pch.h"
#include "ReferenceViewModel.h"
#include "ReferenceViewModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::Foundation;

namespace winrt::FileRenamer::implementation
{
	ReferenceViewModel::ReferenceViewModel()
	{
		_referenceDict.Insert(L"Microsoft.Windows.CppWinRT", L"https://github.com/Microsoft/cppwinrt");
		_referenceDict.Insert(L"Microsoft.Windows.SDK.BuildTools", L"https://aka.ms/WinSDKProjectURL");
		_referenceDict.Insert(L"Mile.Xaml", L"https://github.com/ProjectMile/Mile.Xaml");
	}

	Collections::IMap<hstring, hstring> ReferenceViewModel::ReferenceDict()
	{
		return _referenceDict;
	}
}