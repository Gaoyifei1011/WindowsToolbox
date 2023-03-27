#include "pch.h"
#include "LanguageModel.h"
#if __has_include("LanguageModel.g.cpp")
#include "LanguageModel.g.cpp"
#endif

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
