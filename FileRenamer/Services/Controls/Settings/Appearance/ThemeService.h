#pragma once

#include <tuple>
#include <winrt/base.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.Xaml.h>

#include "global.h"
#include "Extensions/DataType/Constant/ConfigKey.h"
#include "MileWindow.h"
#include "Models/Settings/Appearence/ThemeModel.h"

namespace winrt
{
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtXaml = Windows::UI::Xaml;
}

/// <summary>
/// 应用主题设置服务
/// </summary>
class ThemeService
{
public:
	ThemeService();

	winrt::FileRenamer::ThemeModel DefaultAppTheme();
	void DefaultAppTheme(winrt::FileRenamer::ThemeModel const& value);

	winrt::FileRenamer::ThemeModel AppTheme();
	void AppTheme(winrt::FileRenamer::ThemeModel const& value);

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> ThemeList();
	void ThemeList(winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> const& value);

	void Initialize();
	std::tuple<bool, winrt::FileRenamer::ThemeModel> GetTheme();
	void SetTheme(winrt::FileRenamer::ThemeModel theme, bool isNotFirstSet = true);
	void SetAppTheme();

private:
	winrt::hstring ThemeSettingsKey();

	winrt::hstring _themeSettingsKey;

	winrt::FileRenamer::ThemeModel _defaultAppTheme{ nullptr };
	winrt::FileRenamer::ThemeModel _appTheme{ nullptr };

	winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> _themeList{ nullptr };
};
