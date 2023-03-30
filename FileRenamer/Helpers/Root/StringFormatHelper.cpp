#pragma once

#include "StringFormatHelper.h"

using namespace std;

/// <summary>
/// 对 std::string 类型的字符串进行格式化输出
/// </summary>
template<typename ... Args>
string StringFormatHelper::format(const string& format, Args ... args)
{
	auto size_buf = snprintf(nullptr, 0, format.c_str(), args ...) + 1;
	unique_ptr<char[]> buf(new(nothrow) char[size_buf]);

	if (!buf)
		return string("");

	snprintf(buf.get(), size_buf, format.c_str(), args ...);
	return string(buf.get(), buf.get() + size_buf - 1);
}

/// <summary>
/// 对 std::wstring 类型的字符串进行格式化输出
/// </summary>
template<typename ... Args>
wstring StringFormatHelper::format(const wstring& format, Args ... args)
{
	auto size_buf = snprintf(nullptr, 0, format.c_str(), args ...) + 1;
	unique_ptr<char[]> buf(new(nothrow) char[size_buf]);

	if (!buf)
		return wstring("");

	snprintf(buf.get(), size_buf, format.c_str(), args ...);
	return wstring(buf.get(), buf.get() + size_buf - 1);
}