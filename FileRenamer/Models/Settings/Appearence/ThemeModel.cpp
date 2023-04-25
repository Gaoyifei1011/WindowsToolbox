#include "pch.h"
#include "ThemeModel.h"
#include "ThemeModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThemeModel::ThemeModel(winrt::hstring const& displayName, winrt::hstring const& internalName) :_displayName{ displayName }, _internalName{ internalName } {};

	winrt::hstring ThemeModel::DisplayName()
	{
		return _displayName;
	}
	void ThemeModel::DisplayName(winrt::hstring const& value)
	{
		_displayName = value;
	}

	winrt::hstring ThemeModel::InternalName()
	{
		return _internalName;
	}
	void ThemeModel::InternalName(winrt::hstring const& value)
	{
		_internalName = value;
	}
}