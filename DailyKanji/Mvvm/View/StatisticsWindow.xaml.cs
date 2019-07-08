using DailyKanji.Mvvm.ViewModel;

#nullable enable

namespace DailyKanji.Mvvm.View
{
    /// <summary>
    /// Interaktionslogik für StatisticsWindow.xaml
    /// </summary>
    internal partial class StatisticsWindow
    {
        #region Public Properties

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Constructors

        internal StatisticsWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        #endregion Internal Constructors
    }
}
