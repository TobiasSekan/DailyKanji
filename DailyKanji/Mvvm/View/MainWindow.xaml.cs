using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.ViewModel;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Diagnostics;
using System.Windows.Controls;

#nullable enable

namespace DailyKanji.Mvvm.View
{
    internal partial class MainWindow
    {
        #region Public Properties

        public MainBaseModel BaseModel { get; }

        public MainModel Model { get; }

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Constructors

        internal MainWindow(MainBaseModel baseModel, MainModel model, MainViewModel viewModel)
        {
            BaseModel = baseModel;
            Model     = model;
            ViewModel = viewModel;

            InitializeComponent();

            Model.AnswerElements.Add(new AnswerViewElement(AnswerText01, Button01, AnswerButtonColumn01, AnswerHint01, AnswerKey01));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText02, Button02, AnswerButtonColumn02, AnswerHint02, AnswerKey02));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText03, Button03, AnswerButtonColumn03, AnswerHint03, AnswerKey03));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText04, Button04, AnswerButtonColumn04, AnswerHint04, AnswerKey04));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText05, Button05, AnswerButtonColumn05, AnswerHint05, AnswerKey05));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText06, Button06, AnswerButtonColumn06, AnswerHint06, AnswerKey06));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText07, Button07, AnswerButtonColumn07, AnswerHint07, AnswerKey07));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText08, Button08, AnswerButtonColumn08, AnswerHint08, AnswerKey08));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText09, Button09, AnswerButtonColumn09, AnswerHint09, AnswerKey09));
            Model.AnswerElements.Add(new AnswerViewElement(AnswerText10, Button10, AnswerButtonColumn10, AnswerHint10, AnswerKey10));
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
