#pragma once

#include "winrt/Windows.Foundation.h"
#include "winrt/Windows.Foundation.Collections.h"
#include "winrt/Windows.UI.Xaml.Controls.h"
#include "winrt/Windows.UI.Xaml.Media.Animation.h"
#include "Models/Window/NavigationModel.h"

using namespace winrt;
using namespace winrt::FileRenamer;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::Foundation::Collections;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Media::Animation;

/// <summary>
/// 应用导航服务
/// </summary>
class NavigationService
{
public:
	NavigationService();

	Frame NavigationFrame();
	void NavigationFrame(Frame value);

	IVector<NavigationModel> NavigationItemList();
	void NavigationItemList(IVector<NavigationModel> const& value);
	
	void NavigateTo(TypeName navigationPageType, IInspectable parameter = nullptr);
	void NavigationFrom();
	TypeName GetCurrentPageType();
	bool CanGoBack();

private:
	Frame _navigationFrame{ nullptr };
	SlideNavigationTransitionInfo _navigationTransition{ nullptr };

	IVector<NavigationModel> _navigationItemList{ single_threaded_vector<NavigationModel>() };
};
