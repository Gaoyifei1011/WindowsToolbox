#include "pch.h"
#include "ThanksViewModel.h"
#include "ThanksViewModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThanksViewModel::ThanksViewModel()
	{
		_thanksDict.Append(winrt::make<ThanksKeyValuePairModel>(L"MouriNaruto", L"https://github.com/MouriNaruto"));
	}

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThanksKeyValuePairModel> ThanksViewModel::ThanksDict()
	{
		return _thanksDict;
	}
}