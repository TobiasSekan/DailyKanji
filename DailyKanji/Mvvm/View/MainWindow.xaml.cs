using System.Windows;
using System.Windows.Controls;
using DailyKanji.Mvvm.ViewModel;

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

        private void Button_Click(object sender, RoutedEventArgs e)
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

        #endregion Private Methods
    }
}
