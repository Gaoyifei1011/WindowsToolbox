#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "PrecautionControl.g.h"

namespace winrt::FileRenamer::implementation
{
    struct PrecautionControl : PrecautionControlT<PrecautionControl>
    {
    public:
        PrecautionControl();

        hstring Precaution();

    private:
        hstring _precaution;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct PrecautionControl : PrecautionControlT<PrecautionControl, implementation::PrecautionControl>
    {
    };
}
