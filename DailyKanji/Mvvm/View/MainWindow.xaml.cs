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
    }
}
