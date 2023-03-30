#pragma once

#include <string>
#include <memory>

using namespace std;

/// <summary>
/// 对字符串进行格式化输出辅助类
/// </summary>
class StringFormatHelper
{
public:
	template<typename ... Args>
	string format(const string& format, Args ... args);

	template<typename ... Args>
	wstring format(const wstring& format, Args ... args);
};
