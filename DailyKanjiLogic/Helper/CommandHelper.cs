using System;
using System.Windows.Input;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper to easier work with <see cref="ICommand"/>
    /// </summary>
    public sealed class CommandHelper : ICommand
    {
        #region Private Fields

        /// <summary>
        /// The <see cref="Action{T}"/> for the command
        /// </summary>
        private readonly Action<object> _action;

        /// <summary>
        /// <see cref="Predicate{T}"/> of this command indicate that the action can preform
        /// </summary>
        private readonly Predicate<object> _canExecute;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a new command for the given action
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> for the command</param>
        /// <param name="canExecute">(Optional) The <see cref="Predicate{T}"/> that indicate that the action can preform</param>
        public CommandHelper(in Action<object> action, in Predicate<object> canExecute = null)
        {
            _action      = action;
            _canExecute = canExecute;
        }

        #endregion Public Constructors

        #region ICommand Implementation

        /// <summary>
        /// Execute the <see cref="Action{T}"/> with the given parameter
        /// </summary>
        /// <param name="parameter">The parameter for the action</param>
        public void Execute(object parameter)
            => _action?.Invoke(parameter);

        /// <summary>
        /// Return if the action can perform, based on the <see cref="Predicate{T}"/> of this <see cref="Action{T}"/> and the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true"/> if command is usable, otherwise <see langword="false"/></returns>
        public bool CanExecute(object parameter)
            => _canExecute?.Invoke(parameter) != false;

        public event EventHandler CanExecuteChanged;

        #endregion ICommand Implementation
    }
}
