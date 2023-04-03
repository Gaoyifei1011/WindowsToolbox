#pragma once

#include "pch.h"
#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.Controls.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "NavigationModel.h"
#include "NavigationModel.g.cpp"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Interop;

namespace winrt::FileRenamer::implementation
{
    NavigationModel::NavigationModel() {};

    /// <summary>
    /// 页面导航标签
    /// </summary>
    hstring NavigationModel::NavigationTag()
    {
        return _navigationTag;
    }
    void NavigationModel::NavigationTag(hstring const& value)
    {
        _navigationTag = value;
    }

    /// <summary>
    /// 页面导航控件中项的容器
    /// </summary>
    NavigationViewItem NavigationModel::NavigationItem()
    {
        return _navigationItem;
    }
    void NavigationModel::NavigationItem(NavigationViewItem const& value)
    {
        _navigationItem = value;
    }

    /// <summary>
    /// 页面导航类型
    /// </summary>
    TypeName NavigationModel::NavigationPage()
    {
        return _navigationPage;
    }
    void NavigationModel::NavigationPage(TypeName const& value)
    {
        _navigationPage = value;
    }
}
