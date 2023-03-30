#pragma once

#include <Windows.h>

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"
#include "Helpers/Root/StringFormatHelper.h"
#include "Services/Root/ResourceService.h"

using namespace winrt;
using namespace winrt::FileRenamer;

extern com_ptr<implementation::App> ApplicationRoot;
extern ResourceService AppResourcesService;
extern StringFormatHelper AppStringFormatHelper;