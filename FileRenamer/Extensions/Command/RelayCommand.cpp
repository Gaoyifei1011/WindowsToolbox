#pragma once

#include "RelayCommand.h"

#include <functional>

namespace winrt::FileRenamer::implementation
{
	RelayCommand::RelayCommand(std::function<void(IInspectable)> action)
	{
		m_action = action;
	}

	void RelayCommand::Execute(IInspectable parameter)
	{
		if (m_action != nullptr)
		{
			m_action(parameter);
		}
	}

	/// <summary>
	/// 使用 CanExecute 时要调用的可选操作。
	/// </summary>
	bool RelayCommand::CanExecute(IInspectable parameter)
	{
		return true;
	}

	void RelayCommand::CanExecuteChanged(winrt::event_token const& token) noexcept
	{
		m_eventToken.remove(token);
	}

	winrt::event_token RelayCommand::CanExecuteChanged(EventHandler<IInspectable> const& handler)
	{
		return m_eventToken.add(handler);
	}
}