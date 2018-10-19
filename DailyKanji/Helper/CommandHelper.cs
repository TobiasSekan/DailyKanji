using System;
using System.Windows.Input;

namespace DailyKanji.Helper
{
    public sealed class CommandHelper : ICommand
    {
        private readonly Action<object> _action;

        public CommandHelper(Action<object> action)
            => _action = action;

        public void Execute(object parameter)
            => _action(parameter);

        public bool CanExecute(object parameter)
            => true;

        public event EventHandler CanExecuteChanged;
    }
}
