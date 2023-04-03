#pragma once

#include "winrt/base.h"
#include "winrt/Windows.UI.Xaml.Controls.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "NavigationModel.g.h"

using namespace winrt;
using namespace winrt::Windows::UI::Xaml::Controls;
using namespace winrt::Windows::UI::Xaml::Interop;

namespace winrt::FileRenamer::implementation
{
    struct NavigationModel : NavigationModelT<NavigationModel>
    {
    public:
        NavigationModel();

        hstring NavigationTag();
        void NavigationTag(hstring const& value);

        NavigationViewItem NavigationItem();
        void NavigationItem(NavigationViewItem const& value);

        TypeName NavigationPage();
        void NavigationPage(TypeName const& value);

    private:
        hstring _navigationTag;
        NavigationViewItem _navigationItem;
        TypeName _navigationPage;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct NavigationModel : NavigationModelT<NavigationModel, implementation::NavigationModel>
    {
    };
}
