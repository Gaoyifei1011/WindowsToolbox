#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "InstructionsControl.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
    struct InstructionsControl : InstructionsControlT<InstructionsControl>
    {
    public:
        InstructionsControl();

        hstring UseInstruction();

    private:
        hstring _useInstruction;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct InstructionsControl : InstructionsControlT<InstructionsControl, implementation::InstructionsControl>
    {
    };
}
