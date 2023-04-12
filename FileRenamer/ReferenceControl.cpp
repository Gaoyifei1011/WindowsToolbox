#include "pch.h"
#include "ReferenceControl.h"
#include "ReferenceControl.g.cpp"

namespace winrt::FileRenamer::implementation
{
	ReferenceControl::ReferenceControl()
	{
		InitializeComponent();

		_viewModel = winrt::make<FileRenamer::implementation::ReferenceViewModel>();

		_referenceTitle = AppResourcesService.GetLocalized(L"About/ReferenceTitle");
	}

	winrt::FileRenamer::ReferenceViewModel ReferenceControl::ViewModel()
	{
		return _viewModel;
	}

	winrt::hstring ReferenceControl::ReferenceTitle()
	{
		return _referenceTitle;
	}
}