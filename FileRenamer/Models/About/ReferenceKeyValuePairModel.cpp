#include "pch.h"
#include "ReferenceKeyValuePairModel.h"
#include "ReferenceKeyValuePairModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ReferenceKeyValuePairModel::ReferenceKeyValuePairModel() {};

	ReferenceKeyValuePairModel::ReferenceKeyValuePairModel(winrt::hstring const& key, winrt::hstring const& value) :_key{ key }, _value{ value } {};

	winrt::hstring ReferenceKeyValuePairModel::Key()
	{
		return _key;
	}
	void ReferenceKeyValuePairModel::Key(winrt::hstring const& value)
	{
		_key = value;
	}

	winrt::hstring ReferenceKeyValuePairModel::Value()
	{
		return _value;
	}
	void ReferenceKeyValuePairModel::Value(winrt::hstring const& value)
	{
		_value = value;
	}
}