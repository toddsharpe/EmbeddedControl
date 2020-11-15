using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ground.Framework
{
	public class RelayCommand : ICommand
	{
		private Action<object> _execute;

		private Predicate<object> _canExecute;

		private event EventHandler CanExecuteChangedInternal;

		public RelayCommand(Action<object> execute) : this(execute, DefaultCanExecute)
		{

		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CanExecuteChangedInternal += value;
				OnCanExecuteChanged();
			}

			remove
			{
				CanExecuteChangedInternal -= value;
				OnCanExecuteChanged();
			}
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute != null && _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public void OnCanExecuteChanged()
		{
			//DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
			CanExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
		}

		public void Destroy()
		{
			_canExecute = _ => false;
			_execute = _ => { };
		}

		private static bool DefaultCanExecute(object parameter)
		{
			return true;
		}
	}
}
