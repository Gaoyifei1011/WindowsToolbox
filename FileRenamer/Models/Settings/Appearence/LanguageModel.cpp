#include "pch.h"
#include "LanguageModel.h"
#include "LanguageModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	LanguageModel::LanguageModel() {};

	winrt::hstring LanguageModel::DisplayName()
	{
		return _displayName;
	}
	void LanguageModel::DisplayName(winrt::hstring const& value)
	{
		_displayName = value;
	}

	winrt::hstring LanguageModel::InternalName()
	{
		return _internalName;
	}
	void LanguageModel::InternalName(winrt::hstring const& value)
	{
		_internalName = value;
	}
}