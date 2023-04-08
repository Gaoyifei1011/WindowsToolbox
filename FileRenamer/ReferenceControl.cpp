#include "pch.h"
#include "ReferenceControl.h"
#include "ReferenceControl.g.cpp"
#include "WinMain.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::FileRenamer::implementation
{
	ReferenceControl::ReferenceControl()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::ReferenceViewModel>();

		_reference = AppResourcesService.GetLocalized(L"About/Reference");
	}

	FileRenamer::ReferenceViewModel ReferenceControl::ViewModel()
	{
		return _viewModel;
	}

	hstring ReferenceControl::Reference()
	{
		return _reference;
	}
}