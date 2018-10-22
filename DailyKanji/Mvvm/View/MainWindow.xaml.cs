using DailyKanji.Mvvm.ViewModel;
using System;
using System.Windows.Controls;

namespace DailyKanji.Mvvm.View
{
    internal partial class MainWindow
    {
        #region Public Properties

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Public Constructors

        internal MainWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void Button_Click(object sender, EventArgs e)
        {
            if(!(sender is Button button))
            {
                return;
            }

            if(!(button?.Content is string answer))
            {
                return;
            }

            ViewModel.CheckAnswer(answer);
        }

        private void ComboBox_SelectionChanged(object sender, EventArgs e)
            => ViewModel.ChangeAnswerCount.Execute(null);

        #endregion Private Methods

    }
}
