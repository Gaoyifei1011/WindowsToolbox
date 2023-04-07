#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "ReferenceControl.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
    struct ReferenceControl : ReferenceControlT<ReferenceControl>
    {
    public:
        ReferenceControl();

        hstring Reference();

    private:
        hstring _reference;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ReferenceControl : ReferenceControlT<ReferenceControl, implementation::ReferenceControl>
    {
    };
}
