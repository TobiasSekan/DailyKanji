﻿using System;
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
        private readonly Predicate<object>? _canExecute;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a new command for the given action
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> for the command</param>
        /// <param name="canExecute">(Optional) The <see cref="Predicate{T}"/> that indicate that the action can preform</param>
        public CommandHelper(in Action<object> action, in Predicate<object>? canExecute = null)
        {
            _action      = action;
            _canExecute = canExecute;
        }

        #endregion Public Constructors

        #region ICommand Implementation

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">parameter is null</exception>
        public void Execute(object? parameter)
            => _action?.Invoke(parameter ?? throw new ArgumentNullException(nameof(parameter)));

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
            => parameter is null || _canExecute?.Invoke(parameter) != false;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        #endregion ICommand Implementation
    }
}
