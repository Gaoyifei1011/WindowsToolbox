#include "pch.h"
#include "ThemeModel.h"
#if __has_include("ThemeModel.g.cpp")
#include "ThemeModel.g.cpp"
#endif

namespace winrt::FileRenamer::implementation
{
    int32_t ThemeModel::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void ThemeModel::MyProperty(int32_t /*value*/)
    {
        throw hresult_not_implemented();
    }
}
