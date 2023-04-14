#pragma once

#include <winrt/base.h>
#include <winrt/Windows.Foundation.Collections.h>

#include "Models/About/ThanksKeyValuePairModel.h"
#include "ThanksViewModel.g.h"

namespace winrt
{
    namespace WinrtCollections = Windows::Foundation::Collections;
}

namespace winrt::FileRenamer::implementation
{
    struct ThanksViewModel : ThanksViewModelT<ThanksViewModel>
    {
    public:
        ThanksViewModel();

        winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThanksKeyValuePairModel> ThanksDict();

    private:
        winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThanksKeyValuePairModel> _thanksDict{ winrt::single_threaded_observable_vector<winrt::FileRenamer::ThanksKeyValuePairModel>() };
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ThanksViewModel : ThanksViewModelT<ThanksViewModel, implementation::ThanksViewModel>
    {
    };
}
