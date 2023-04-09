#pragma once

#include "winrt/base.h"
#include "ReferenceKeyValuePairModel.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
    struct ReferenceKeyValuePairModel : ReferenceKeyValuePairModelT<ReferenceKeyValuePairModel>
    {
    public:
        ReferenceKeyValuePairModel();
        ReferenceKeyValuePairModel(hstring const& key, hstring const& value);

        hstring Key();
        void Key(hstring const& value);

        hstring Value();
        void Value(hstring const& value);

    private:
        hstring _key;
        hstring _value;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ReferenceKeyValuePairModel : ReferenceKeyValuePairModelT<ReferenceKeyValuePairModel, implementation::ReferenceKeyValuePairModel>
    {
    };
}
