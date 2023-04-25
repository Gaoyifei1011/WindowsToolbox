#include "pch.h"
#include "ThanksKeyValuePairModel.h"
#include "ThanksKeyValuePairModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThanksKeyValuePairModel::ThanksKeyValuePairModel(winrt::hstring const& key, winrt::hstring const& value) :_key{ key }, _value{ value } {};

	winrt::hstring ThanksKeyValuePairModel::Key()
	{
		return _key;
	}
	void ThanksKeyValuePairModel::Key(winrt::hstring const& value)
	{
		_key = value;
	}

	winrt::hstring ThanksKeyValuePairModel::Value()
	{
		return _value;
	}
	void ThanksKeyValuePairModel::Value(winrt::hstring const& value)
	{
		_value = value;
	}
}