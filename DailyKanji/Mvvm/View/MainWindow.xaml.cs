﻿using DailyKanji.Mvvm.ViewModel;
using System.Collections.Generic;
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

        internal IReadOnlyList<TextBlock> _answerTextList { get; }

        internal IReadOnlyList<ButtonBase> _buttonList { get; }

        internal IReadOnlyList<ColumnDefinition> _answerButtonColumn { get; }

        internal IReadOnlyList<TextBlock> _answerHintTextBlock { get; }

        internal IReadOnlyList<TextBlock> _answerShortCutTextBlock { get; }

        #endregion Internal Properties

        #region Public Constructors

        internal MainWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            _answerTextList = new[]
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

            _buttonList = new[]
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

            _answerButtonColumn = new[]
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

            _answerHintTextBlock = new[]
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

            _answerShortCutTextBlock = new[]
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

        #endregion Public Constructors
    }
}
