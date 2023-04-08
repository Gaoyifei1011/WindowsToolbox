#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "ThanksControl.g.h"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl>
	{
	public:
		ThanksControl();

		hstring Thanks();

	private:
		hstring _thanks;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ThanksControl : ThanksControlT<ThanksControl, implementation::ThanksControl>
	{
	};
}
