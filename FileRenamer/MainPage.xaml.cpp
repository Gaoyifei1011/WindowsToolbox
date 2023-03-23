#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;;

namespace winrt::FileRenamer::implementation
{
    MainPage::MainPage()
    {
        InitializeComponent();
    }

    int32_t MainPage::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void MainPage::MyProperty(int32_t /* value */)
    {
        throw hresult_not_implemented();
    }

    void MainPage::ClickHandler(IInspectable const&, RoutedEventArgs const&)
    {
        Button().Content(box_value(L"Clicked"));
    }
}
