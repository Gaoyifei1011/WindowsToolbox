#pragma once

#include <Windows.h>

#include "pch.h"
#include "App.h"
#include "MainPage.h"
#include "Helpers/Root/StringFormatHelper.h"
#include "Services/Root/ResourceService.h"
#include "Services/Window/NavigationService.h"

using namespace winrt;
using namespace winrt::FileRenamer;

extern com_ptr<implementation::App> ApplicationRoot;
extern ResourceService AppResourcesService;
extern NavigationService AppNavigationService;
extern StringFormatHelper AppStringFormatHelper;