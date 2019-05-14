using DailyKanji.Mvvm.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DailyKanji.Mvvm.View
{
    internal partial class MainWindow
    {
        #region Public Properties

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Properties

        internal IReadOnlyList<TextBlock> AnswerTextList { get; }

        internal IReadOnlyList<ButtonBase> ButtonList { get; }

        internal IReadOnlyList<ColumnDefinition> AnswerButtonColumn { get; }

        internal IReadOnlyList<TextBlock> AnswerHintTextBlock { get; }

        internal IReadOnlyList<TextBlock> AnswerShortCutTextBlock { get; }

        #endregion Internal Properties

        #region Internal Constructors

        internal MainWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            AnswerTextList = new[]
            {
                AnswerText01,
                AnswerText02,
                AnswerText03,
                AnswerText04,
                AnswerText05,
                AnswerText06,
                AnswerText07,
                AnswerText08,
                AnswerText09,
                AnswerText10
            };

            ButtonList = new[]
            {
                Button01,
                Button02,
                Button03,
                Button04,
                Button05,
                Button06,
                Button07,
                Button08,
                Button09,
                Button10
            };

            AnswerButtonColumn = new[]
            {
                AnswerButtonColumn01,
                AnswerButtonColumn02,
                AnswerButtonColumn03,
                AnswerButtonColumn04,
                AnswerButtonColumn05,
                AnswerButtonColumn06,
                AnswerButtonColumn07,
                AnswerButtonColumn08,
                AnswerButtonColumn09,
                AnswerButtonColumn10
            };

            AnswerHintTextBlock = new[]
            {
                AnswerHint01,
                AnswerHint02,
                AnswerHint03,
                AnswerHint04,
                AnswerHint05,
                AnswerHint06,
                AnswerHint07,
                AnswerHint08,
                AnswerHint09,
                AnswerHint10
            };

            AnswerShortCutTextBlock = new[]
            {
                AnswerKey01,
                AnswerKey02,
                AnswerKey03,
                AnswerKey04,
                AnswerKey05,
                AnswerKey06,
                AnswerKey07,
                AnswerKey08,
                AnswerKey09,
                AnswerKey10
            };
        }

        #endregion Internal Constructors

        # region Private Methods

        /// <summary>
        /// Command redirection for right-click on a answer button
        /// </summary>
        /// <param name="sender">The sender <see cref="object"/> of the right-click</param>
        /// <param name="e">The arguments of this events (not used)</param>
        private void HighlightAnswer(object sender, EventArgs e)
        {
            if(!(sender is Button button))
            {
                Debug.Fail("Button not found");
                return;
            }

            ViewModel.CommandHighlightAnswer.Execute(button.Name.Replace("Button", string.Empty));
        }

        /// <summary>
        /// Restart the answer timer after the answer timeout has changed
        /// </summary>
        /// <param name="sender">The sender of this event (not used)</param>
        /// <param name="e">The arguments of this event (not used)</param>
        private void ChangeAnswerTimer(object sender, EventArgs e)
            => ViewModel.RestartTestTimer();

        #endregion Private Methods

    }
}
