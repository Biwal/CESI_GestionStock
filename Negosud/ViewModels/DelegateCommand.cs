using System;
using System.Windows.Input;

namespace Negosud.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _Execute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _canExecute = canExecute;
            _Execute = execute;
        }

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        { }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _Execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
