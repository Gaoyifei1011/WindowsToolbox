#pragma once

//#include <objbase.h>
//#include <shellapi.h>
//#pragma comment(lib,"shell32.lib")
#include <windows.h>
//#include <ShlObj.h>
//#include <unknwn.h>
//#include <string>

#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.Core.h>
#include <winrt/Windows.UI.StartScreen.h>
#include <winrt/Windows.System.h>
#include <Extensions/Command/RelayCommand.h>
#include <AboutViewModel.g.h>

using namespace std;

namespace winrt::FileRenamer::implementation
{
    struct AboutViewModel : AboutViewModelT<AboutViewModel>
    {
    public:
        AboutViewModel();

        ICommand CreateDesktopShortcutCommand();
        ICommand PinToStartScreenCommand();
        ICommand PinToTaskbarCommand();
        ICommand ShowReleaseNotesCommand();
        ICommand ShowLicenseCommand();

    private:
        ICommand _createDesktopShortcutCommand;
        ICommand _pinToStartScreenCommand;
        ICommand _pinToTaskbarCommand;
        ICommand _showReleaseNotesCommand;
        ICommand _showLicenseCommand;
    };
}

namespace winrt::FileRenamer::factory_implementation
{
    struct AboutViewModel : AboutViewModelT<AboutViewModel, implementation::AboutViewModel>
    {
    };
}
