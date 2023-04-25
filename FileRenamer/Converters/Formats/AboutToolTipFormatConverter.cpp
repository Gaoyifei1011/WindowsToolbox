#include "pch.h"
#include "AboutToolTipFormatConverter.h"
#include "AboutToolTipFormatConverter.g.cpp"

namespace winrt::FileRenamer::implementation
{
	AboutToolTipFormatConverter::AboutToolTipFormatConverter() {}

	winrt::WinrtFoundation::IInspectable AboutToolTipFormatConverter::Convert(winrt::WinrtFoundation::IInspectable const& value, winrt::WinrtInterop::TypeName const& targetType, winrt::WinrtFoundation::IInspectable const& parameter, winrt::hstring const& language)
	{
		if (value == nullptr || parameter == nullptr)
		{
			return winrt::box_value(L"");
		}

		winrt::hstring param = winrt::unbox_value<winrt::hstring>(parameter);

		if (param == L"Reference")
		{
			return winrt::box_value(winrt::unbox_value<winrt::hstring>(value) + L"\n" + AppResourceService.GetLocalized(L"About/ReferenceToolTip"));
		}
		else if (param == L"Thanks")
		{
			return winrt::box_value(winrt::unbox_value<winrt::hstring>(value) + L"\n" + AppResourceService.GetLocalized(L"About/ThanksToolTip"));
		}
		else
		{
			return value;
		}
	}
	winrt::WinrtFoundation::IInspectable AboutToolTipFormatConverter::ConvertBack(winrt::WinrtFoundation::IInspectable const& value, winrt::WinrtInterop::TypeName const& targetType, winrt::WinrtFoundation::IInspectable const& parameter, winrt::hstring const& language)
	{
		return winrt::WinrtFoundation::IInspectable();
	}
	;
}