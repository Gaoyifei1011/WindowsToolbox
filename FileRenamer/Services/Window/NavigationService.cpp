#include "winrt/Windows.UI.Xaml.Media.Animation.h"
#include "NavigationService.h"

using namespace winrt::Windows::UI::Xaml::Media::Animation;

NavigationService::NavigationService() {};

Frame NavigationService::NavigationFrame()
{
	return _navigationFrame;
}

void NavigationService::NavigationFrame(Frame value)
{
	_navigationFrame = value;
}

IVector<NavigationModel> NavigationService::NavigationItemList()
{
	return _navigationItemList;
}

void NavigationService::NavigationItemList(IVector<NavigationModel> const& value)
{
	_navigationItemList = value;
}

/// <summary>
 /// 页面向前导航
 /// </summary>
void NavigationService::NavigateTo(TypeName navigationPageType, IInspectable parameter)
{
	for (uint32_t index = 0; index < NavigationService::NavigationItemList().Size(); index++)
	{
		if (NavigationService::NavigationItemList().GetAt(index).NavigationPage() == navigationPageType)
		{
			SlideNavigationTransitionInfo info;
			info.Effect(SlideNavigationTransitionEffect::FromRight);
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
TypeName NavigationService::GetCurrentPageType()
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