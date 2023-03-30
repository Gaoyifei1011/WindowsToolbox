#include "pch.h"
#include "LanguageModel.h"
#include "LanguageModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	LanguageModel::LanguageModel() {};

	hstring LanguageModel::DisplayName()
	{
		return _displayName;
	}
	void LanguageModel::DisplayName(hstring const& value)
	{
		_displayName = value;
	}

	hstring LanguageModel::InternalName()
	{
		return _internalName;
	}
	void LanguageModel::InternalName(hstring const& value)
	{
		_internalName = value;
	}
}
