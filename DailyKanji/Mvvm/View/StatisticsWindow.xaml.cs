using DailyKanji.Mvvm.ViewModel;
using DailyKanjiLogic.Mvvm.Model;

#nullable enable

namespace DailyKanji.Mvvm.View
{
    internal partial class StatisticsWindow
    {
        #region Public Properties

        /// <summary>
        /// A data model that contain all data for the program logic and all Kanji data
        /// </summary>
        public MainBaseModel Model { get; }

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsWindow"/> class
        /// </summary>
        /// <param name="model">A data model that contain all data for the program logic and all Kanji data</param>
        /// <param name="viewModel"></param>
        internal StatisticsWindow(MainBaseModel model, MainViewModel viewModel)
        {
            Model     = model;
            ViewModel = viewModel;

            InitializeComponent();
        }

        #endregion Internal Constructors
    }
}
