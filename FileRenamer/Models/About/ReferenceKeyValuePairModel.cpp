#include "pch.h"
#include "ReferenceKeyValuePairModel.h"
#include "ReferenceKeyValuePairModel.g.cpp"

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
	ReferenceKeyValuePairModel::ReferenceKeyValuePairModel() {};

	ReferenceKeyValuePairModel::ReferenceKeyValuePairModel(hstring const& key, hstring const& value) :_key{ key }, _value{ value } {};

	hstring ReferenceKeyValuePairModel::Key()
	{
		return _key;
	}
	void ReferenceKeyValuePairModel::Key(hstring const& value)
	{
		_key = value;
	}

	hstring ReferenceKeyValuePairModel::Value()
	{
		return _value;
	}
	void ReferenceKeyValuePairModel::Value(hstring const& value)
	{
		_value = value;
	}
}
