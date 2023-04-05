#pragma once

#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "RestartContentDialog.g.h"

namespace winrt::FileRenamer::implementation
{
	struct RestartContentDialog : RestartContentDialogT<RestartContentDialog>
	{
		RestartContentDialog()
		{
			// Xaml objects should not call InitializeComponent during construction.
			// See https://github.com/microsoft/cppwinrt/tree/master/nuget#initializecomponent
		}
	};
}

namespace winrt::FileRenamer::factory_implementation
{
	struct RestartContentDialog : RestartContentDialogT<RestartContentDialog, implementation::RestartContentDialog>
	{
	};
}
