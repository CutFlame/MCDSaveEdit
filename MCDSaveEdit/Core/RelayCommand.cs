using System;
using System.Windows.Input;

namespace MCDSaveEdit
{
    public class RelayCommand<TParam> : ICommand where TParam : class
    {
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        private readonly Action<TParam> _methodToExecute;

        public RelayCommand(Action<TParam> methodToExecute)
        {
            _methodToExecute = methodToExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _methodToExecute?.Invoke(parameter as TParam);
        }
    }
}
