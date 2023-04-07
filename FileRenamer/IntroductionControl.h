#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "IntroductionControl.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
    struct IntroductionControl : IntroductionControlT<IntroductionControl>
    {
    public:
        IntroductionControl();

        hstring BriefIntroduction();

    private:
        hstring _briefIntroduction;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct IntroductionControl : IntroductionControlT<IntroductionControl, implementation::IntroductionControl>
    {
    };
}
