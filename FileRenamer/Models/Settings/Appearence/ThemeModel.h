#pragma once

#include "ThemeModel.g.h"

namespace winrt::FileRenamer::implementation
{
	struct ThemeModel : ThemeModelT<ThemeModel>
	{
	public:
		ThemeModel(winrt::hstring const& key, winrt::hstring const& value);

		winrt::hstring DisplayName();
		void DisplayName(winrt::hstring const& value);

		winrt::hstring InternalName();
		void InternalName(winrt::hstring const& value);

	private:
		winrt::hstring _displayName;
		winrt::hstring _internalName;
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct ThemeModel : ThemeModelT<ThemeModel, implementation::ThemeModel>
	{
	};
}
