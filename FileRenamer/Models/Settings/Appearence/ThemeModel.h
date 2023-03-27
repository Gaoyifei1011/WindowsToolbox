#pragma once

#include "ThemeModel.g.h"

namespace winrt::FileRenamer::implementation
{
    struct ThemeModel : ThemeModelT<ThemeModel>
    {
        ThemeModel() = default;

        int32_t MyProperty();
        void MyProperty(int32_t value);
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct ThemeModel : ThemeModelT<ThemeModel, implementation::ThemeModel>
    {
    };
}
