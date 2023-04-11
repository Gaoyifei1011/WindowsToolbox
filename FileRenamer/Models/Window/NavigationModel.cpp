#include "pch.h"
#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.Controls.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "NavigationModel.h"
#include "NavigationModel.g.cpp"

namespace winrt::FileRenamer::implementation
{
	NavigationModel::NavigationModel() {};

	/// <summary>
	/// 页面导航标签
	/// </summary>
	winrt::hstring NavigationModel::NavigationTag()
	{
		return _navigationTag;
	}
	void NavigationModel::NavigationTag(winrt::hstring const& value)
	{
		_navigationTag = value;
	}

	/// <summary>
	/// 页面导航控件中项的容器
	/// </summary>
	winrt::WinrtControls::NavigationViewItem NavigationModel::NavigationItem()
	{
		return _navigationItem;
	}
	void NavigationModel::NavigationItem(winrt::WinrtControls::NavigationViewItem const& value)
	{
		_navigationItem = value;
	}

	/// <summary>
	/// 页面导航类型
	/// </summary>
	winrt::WinrtInterop::TypeName NavigationModel::NavigationPage()
	{
		return _navigationPage;
	}
	void NavigationModel::NavigationPage(winrt::WinrtInterop::TypeName const& value)
	{
		_navigationPage = value;
	}
}