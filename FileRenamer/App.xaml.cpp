#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

#include <Mile.Xaml.h>

using namespace winrt;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace FileRenamer;
using namespace FileRenamer::implementation;



namespace winrt::FileRenamer::implementation
{
    App::App()
    {
        ::MileXamlGlobalInitialize();
    }

    void App::Close()
    {
        Exit();
        ::MileXamlGlobalUninitialize();
    }
}
