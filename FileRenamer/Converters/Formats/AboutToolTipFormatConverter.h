#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Interop.h>
#include <WinMain.h>

#include "AboutToolTipFormatConverter.g.h"

namespace winrt
{
    namespace WinrtFoundation = Windows::Foundation;
    namespace WinrtInterop = Windows::UI::Xaml::Interop;
}

namespace winrt::FileRenamer::implementation
{
    /// <summary>
    /// 关于界面项目引用和感谢介绍文字提示转换器
    /// </summary>
    struct AboutToolTipFormatConverter : AboutToolTipFormatConverterT<AboutToolTipFormatConverter>
    {
        AboutToolTipFormatConverter();

        winrt::WinrtFoundation::IInspectable Convert(
            winrt::WinrtFoundation::IInspectable const& value,
            winrt::WinrtInterop::TypeName const& targetType,
            winrt::WinrtFoundation::IInspectable const& parameter,
            winrt::hstring const& language
            );

        winrt::WinrtFoundation::IInspectable ConvertBack(
            winrt::WinrtFoundation::IInspectable const& value,
            winrt::WinrtInterop::TypeName const& targetType,
            winrt::WinrtFoundation::IInspectable const& parameter,
            winrt::hstring const& language
        );
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct AboutToolTipFormatConverter : AboutToolTipFormatConverterT<AboutToolTipFormatConverter, implementation::AboutToolTipFormatConverter>
    {
    };
}
