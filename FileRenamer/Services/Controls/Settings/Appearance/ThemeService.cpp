#include "ThemeService.h"

ThemeService::ThemeService()
{
	_themeSettingsKey = ConfigKey::ThemeKey;
}

winrt::FileRenamer::ThemeModel ThemeService::DefaultAppTheme()
{
	return _defaultAppTheme;
}
void ThemeService::DefaultAppTheme(winrt::FileRenamer::ThemeModel const& value)
{
	_defaultAppTheme = value;
}

winrt::FileRenamer::ThemeModel ThemeService::AppTheme()
{
	return _appTheme;
}
void ThemeService::AppTheme(winrt::FileRenamer::ThemeModel const& value)
{
	_appTheme = value;
}

winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> ThemeService::ThemeList()
{
	return _themeList;
}
void ThemeService::ThemeList(winrt::WinrtCollections::IObservableVector<winrt::FileRenamer::ThemeModel> const& value)
{
	_themeList = value;
}

/// <summary>
/// 应用在初始化前获取设置存储的主题值
/// </summary>
void ThemeService::Initialize()
{
	ThemeService::ThemeList(AppResourceService.ThemeList());

	for (uint32_t index = 0; index < ThemeService::ThemeList().Size(); index++)
	{
		if (ThemeService::ThemeList().GetAt(index).InternalName() == L"Default")
		{
			ThemeService::DefaultAppTheme(ThemeService::ThemeList().GetAt(index));
		}
	}

	std::tuple<bool, winrt::FileRenamer::ThemeModel> ThemeResult = ThemeService::GetTheme();

	ThemeService::AppTheme(std::get<1>(ThemeResult));

	if (std::get<0>(ThemeResult))
	{
		ThemeService::SetTheme(ThemeService::AppTheme(), false);
	}
}

/// <summary>
/// 获取设置存储的主题值，如果设置没有存储，使用默认值
/// </summary>
std::tuple<bool, winrt::FileRenamer::ThemeModel> ThemeService::GetTheme()
{
	auto theme = AppConfigService.ReadSettings(ThemeService::ThemeSettingsKey());

	if (theme == nullptr)
	{
		uint32_t themeIndex = 0;
		for (uint32_t index = 0; index < ThemeService::ThemeList().Size(); index++)
		{
			if (ThemeService::ThemeList().GetAt(index).InternalName() == ThemeService::DefaultAppTheme().InternalName())
			{
				themeIndex = index;
			}
		}

		return std::tuple<bool, winrt::FileRenamer::ThemeModel>(true, ThemeService::ThemeList().GetAt(themeIndex));
	}
	std::tuple<bool, winrt::FileRenamer::ThemeModel> test(false, winrt::make<winrt::FileRenamer::implementation::ThemeModel>(L"test1", L"test2"));
	return test;
}

/// <summary>
/// 应用主题发生修改时修改设置存储的主题值
/// </summary>
void ThemeService::SetTheme(winrt::FileRenamer::ThemeModel theme, bool isNotFirstSet)
{
	if (isNotFirstSet)
	{
		ThemeService::AppTheme(theme);
	}

	AppConfigService.SaveSettings(ThemeService::ThemeSettingsKey(), winrt::box_value(theme.InternalName()));
}

/// <summary>
/// 设置应用显示的主题
/// </summary>
void ThemeService::SetAppTheme()
{
	winrt::WinrtXaml::FrameworkElement frameworkElement = MileWindow::Current()->Content().try_as<winrt::WinrtXaml::FrameworkElement>();
	if (frameworkElement != nullptr)
	{
		if (ThemeService::AppTheme().InternalName() == L"Default")
		{
			frameworkElement.RequestedTheme(winrt::WinrtXaml::ElementTheme::Default);
		}
		else if (ThemeService::AppTheme().InternalName() == L"Light")
		{
			frameworkElement.RequestedTheme(winrt::WinrtXaml::ElementTheme::Light);
		}
		else if (ThemeService::AppTheme().InternalName() == L"Dark")
		{
			frameworkElement.RequestedTheme(winrt::WinrtXaml::ElementTheme::Dark);
		}
	}
}

winrt::hstring ThemeService::ThemeSettingsKey()
{
	return _themeSettingsKey;
}