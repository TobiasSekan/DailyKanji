using DailyKanji.Mvvm.ViewModel;

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

        public StatisticsWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        #endregion Internal Constructors
    }
}
