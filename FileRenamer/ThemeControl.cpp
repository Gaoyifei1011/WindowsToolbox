#include "pch.h"
#include "ThemeControl.h"
#include "ThemeControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ThemeControl::ThemeControl()
	{
		InitializeComponent();

		_viewModel = winrt::make<winrt::FileRenamer::implementation::ThemeViewModel>();

		_themeMode = AppResourcesService.GetLocalized(L"Settings/ThemeMode");
		_themeModeDescription = AppResourcesService.GetLocalized(L"Settings/ThemeModeDescription");
		_windowsColorSettings = AppResourcesService.GetLocalized(L"Settings/WindowsColorSettings");
	}

	winrt::FileRenamer::ThemeViewModel ThemeControl::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring ThemeControl::ThemeMode()
	{
		return _themeMode;
	}

	winrt::hstring ThemeControl::ThemeModeDescription()
	{
		return _themeModeDescription;
	}

	winrt::hstring ThemeControl::WindowsColorSettings()
	{
		return _windowsColorSettings;
	}
}
