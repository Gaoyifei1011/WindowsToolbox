#include "pch.h"
#include "ReferenceControl.h"
#include "ReferenceControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ReferenceControl::ReferenceControl()
	{
		InitializeComponent();

		_viewModel = make<FileRenamer::implementation::ReferenceViewModel>();

		_reference = AppResourcesService.GetLocalized(L"About/Reference");
	}

	winrt::FileRenamer::ReferenceViewModel ReferenceControl::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring ReferenceControl::Reference()
	{
		return _reference;
	}
}