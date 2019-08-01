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
