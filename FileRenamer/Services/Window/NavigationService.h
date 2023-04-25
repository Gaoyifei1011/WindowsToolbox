#pragma once

#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <winrt/Windows.UI.Xaml.Controls.h>
#include <winrt/Windows.UI.Xaml.Media.Animation.h>

#include "Models/Window/NavigationModel.h"

namespace winrt
{
	namespace WinrtAnimation = Windows::UI::Xaml::Media::Animation;
	namespace WinrtControls = Windows::UI::Xaml::Controls;
	namespace WinrtCollections = Windows::Foundation::Collections;
	namespace WinrtFoundation = Windows::Foundation;
	namespace WinrtInterop = Windows::UI::Xaml::Interop;
}

/// <summary>
/// 应用导航服务
/// </summary>
class NavigationService
{
public:
	NavigationService();

	winrt::WinrtControls::Frame NavigationFrame();
	void NavigationFrame(winrt::WinrtControls::Frame value);

	winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> NavigationItemList();
	void NavigationItemList(winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> const& value);

	void NavigateTo(winrt::WinrtInterop::TypeName navigationPageType, winrt::WinrtFoundation::IInspectable parameter = nullptr);
	void NavigationFrom();
	winrt::WinrtInterop::TypeName GetCurrentPageType();
	bool CanGoBack();

private:
	winrt::WinrtControls::Frame _navigationFrame{ nullptr };

	winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> _navigationItemList{ winrt::single_threaded_vector<winrt::FileRenamer::NavigationModel>() };
};
