#pragma once

#include <Windows.h>

#include "App.xaml.g.h"
#include "MileWindow.h"

namespace winrt::FileRenamer::implementation
{
	class App : public AppT<App>
	{
	public:
		App();
		MileWindow MainWindow;
		void Run(HINSTANCE hInstance, int nShowCmd);
		void CloseApp();
	};
}
