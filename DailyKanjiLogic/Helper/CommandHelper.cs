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
        /// <see cref="Action"/> of this command
        /// </summary>
        private readonly Action _action;

        /// <summary>
        /// <see cref="Action{}"/> of this command
        /// </summary>
        private readonly Action<object> _actionWithParameter;

        /// <summary>
        /// <see cref="Predicate{}"/> of this command indicate that the action can preform
        /// </summary>
        private readonly Predicate<bool> _canExcecute;

        /// <summary>
        /// <see cref="Predicate{}"/> of this command indicate that the action can preform
        /// </summary>
        private readonly Predicate<object> _canExcecuteWithParameter;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a new command for the given action
        /// </summary>
        /// <param name="action">The <see cref="Action"/> for the command</param>
        /// <param name="canExcecute">(Optional) The <see cref="Predicate{}"/> that indicate that the action can preform</param>
        public CommandHelper(in Action action, in Predicate<bool> canExcecute = null)
        {
            _action      = action;
            _canExcecute = canExcecute;
        }

        /// <summary>
        /// Create a new command for the given action
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> for the command</param>
        /// <param name="canExcecute">(Optional) The <see cref="Predicate{T}"/> that indicate that the action can preform</param>
        public CommandHelper(in Action<object> action, in Predicate<object> canExcecute = null)
        {
            _actionWithParameter      = action;
            _canExcecuteWithParameter = canExcecute;
        }

        #endregion Public Constructors

        #region ICommand Implementation

        /// <summary>
        /// Execute the <see cref="Action{T}"/> with the given parameter
        /// </summary>
        /// <param name="parameter">The parameter for the action</param>
        public void Execute(object parameter)
        {
            _action?.Invoke();

            _actionWithParameter?.Invoke(parameter);
        }

        /// <summary>
        /// Return if the action can perform, based on the <see cref="Predicate{T}"/> of this <see cref="Action{T}"/> and the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><c>true</c> if command is usable, otherwise <c>false</c></returns>
        public bool CanExecute(object parameter)
            => _canExcecute != null
                ? bool.TryParse(parameter.ToString(), out var parameterBool) && _canExcecute(Convert.ToBoolean(parameterBool))
                : _canExcecuteWithParameter == null || _canExcecuteWithParameter(parameter);

        public event EventHandler CanExecuteChanged;

        #endregion ICommand Implementation
    }
}
