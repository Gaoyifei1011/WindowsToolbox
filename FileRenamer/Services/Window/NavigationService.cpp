#include "NavigationService.h"

NavigationService::NavigationService() {};

winrt::WinrtControls::Frame NavigationService::NavigationFrame()
{
	return _navigationFrame;
}

void NavigationService::NavigationFrame(winrt::WinrtControls::Frame value)
{
	_navigationFrame = value;
}

winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> NavigationService::NavigationItemList()
{
	return _navigationItemList;
}

void NavigationService::NavigationItemList(winrt::WinrtCollections::IVector<winrt::FileRenamer::NavigationModel> const& value)
{
	_navigationItemList = value;
}

/// <summary>
 /// 页面向前导航
 /// </summary>
void NavigationService::NavigateTo(winrt::WinrtInterop::TypeName navigationPageType, winrt::WinrtFoundation::IInspectable parameter)
{
	for (uint32_t index = 0; index < NavigationService::NavigationItemList().Size(); index++)
	{
		if (NavigationService::NavigationItemList().GetAt(index).NavigationPage() == navigationPageType)
		{
			winrt::WinrtAnimation::SlideNavigationTransitionInfo info;
			info.Effect(winrt::WinrtAnimation::SlideNavigationTransitionEffect::FromRight);
			NavigationService::NavigationFrame().Navigate(
				NavigationService::NavigationItemList().GetAt(index).NavigationPage(),
				parameter,
				info
			);
		}
	}
}

/// <summary>
/// 页面向后导航
/// </summary>
void NavigationService::NavigationFrom()
{
	if (NavigationService::NavigationFrame().CanGoBack())
	{
		NavigationService::NavigationFrame().GoBack();
	}
}

/// <summary>
/// 获取当前导航到的页
/// </summary>
winrt::WinrtInterop::TypeName NavigationService::GetCurrentPageType()
{
	return NavigationService::NavigationFrame().CurrentSourcePageType();
}

/// <summary>
/// 检查当前页面是否能向后导航
/// </summary>
bool NavigationService::CanGoBack()
{
	return NavigationService::NavigationFrame().CanGoBack();
}