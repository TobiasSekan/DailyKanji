using System;
using System.Windows.Input;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper to easier work with <see cref="ICommand"/>
    /// </summary>
    public sealed class CommandHelperSlim : ICommand
    {
        #region Private Fields

        /// <summary>
        /// The <see cref="Action"/> for the command
        /// </summary>
        private readonly Action _action;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a new command for the given action
        /// </summary>
        /// <param name="action">The <see cref="Action"/> for the command</param>
        public CommandHelperSlim(in Action action)
            => _action = action;

        #endregion Public Constructors

        #region ICommand Implementation

        /// <inheritdoc/>
        public void Execute(object? parameter)
            => _action?.Invoke();

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
            => true;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        #endregion ICommand Implementation
    }
}
