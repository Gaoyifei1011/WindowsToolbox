#pragma once

#include <functional>
#include <winrt/base.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.UI.Xaml.Input.h>

using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::UI::Xaml::Input;

namespace winrt::FileRenamer::implementation
{
	/// <summary>
	///  一个命令，其唯一用途是通过调用委托将其功能中继到其他对象。
	/// </summary>
	struct RelayCommand : winrt::implements<RelayCommand, ICommand>
	{
		RelayCommand(std::function<void(IInspectable)> action);

		void Execute(IInspectable parameter);
		bool CanExecute(IInspectable parameter);
		void CanExecuteChanged(winrt::event_token const& token) noexcept;

		winrt::event_token CanExecuteChanged(EventHandler<IInspectable> const& handler);

	private:
		std::function<void(IInspectable)> m_action;
		winrt::event<EventHandler<IInspectable>> m_eventToken;
	};
}
