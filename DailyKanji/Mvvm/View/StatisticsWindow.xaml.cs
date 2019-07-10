using DailyKanji.Mvvm.ViewModel;
using DailyKanjiLogic.Mvvm.Model;

#nullable enable

namespace DailyKanji.Mvvm.View
{
    internal partial class StatisticsWindow
    {
        #region Public Properties

        public MainBaseModel Model { get; }

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Constructors

        internal StatisticsWindow(MainBaseModel model, MainViewModel viewModel)
        {
            Model     = model;
            ViewModel = viewModel;

            InitializeComponent();
        }

        #endregion Internal Constructors
    }
}
