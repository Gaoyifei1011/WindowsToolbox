#include "RelayCommand.h"

namespace winrt::FileRenamer::implementation
{
	RelayCommand::RelayCommand(std::function<void(winrt::WinrtFoundation::IInspectable)> action)
	{
		m_action = action;
	}

	void RelayCommand::Execute(winrt::WinrtFoundation::IInspectable parameter)
	{
		if (m_action != nullptr)
		{
			m_action(parameter);
		}
	}

	/// <summary>
	/// 使用 CanExecute 时要调用的可选操作。
	/// </summary>
	bool RelayCommand::CanExecute(winrt::WinrtFoundation::IInspectable parameter)
	{
		return true;
	}

	void RelayCommand::CanExecuteChanged(winrt::event_token const& token) noexcept
	{
		m_eventToken.remove(token);
	}

	winrt::event_token RelayCommand::CanExecuteChanged(winrt::WinrtFoundation::EventHandler<winrt::WinrtFoundation::IInspectable> const& handler)
	{
		return m_eventToken.add(handler);
	}
}