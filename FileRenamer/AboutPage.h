#pragma once

#include "AboutPage.g.h"

namespace winrt::FileRenamer::implementation
{
    struct AboutPage : AboutPageT<AboutPage>
    {
        AboutPage();
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct AboutPage : AboutPageT<AboutPage, implementation::AboutPage>
    {
    };
}
