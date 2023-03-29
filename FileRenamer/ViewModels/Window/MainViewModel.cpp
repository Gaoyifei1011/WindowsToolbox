#pragma once

#include "pch.h"
#include "MainViewModel.h"
#if __has_include("MainViewModel.g.cpp")
#include "MainViewModel.g.cpp"
#endif

using namespace winrt;

namespace winrt::FileRenamer::implementation
{
    MainViewModel::MainViewModel() 
    {
        _clickCommand = make<RelayCommand>([this](IInspectable parameter)
            {
                TCHAR szFullPath[MAX_PATH];
                ZeroMemory(szFullPath, MAX_PATH);
                GetModuleFileName(NULL, szFullPath, MAX_PATH);
                MessageBox(ApplicationRoot->MainWindow.Handle(), L"测试对话框", szFullPath, MB_OK);
            });
    };

    ICommand MainViewModel::ClickCommand()
    {
        return _clickCommand;
    }
}
