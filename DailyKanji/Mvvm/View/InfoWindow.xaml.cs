using DailyKanji.Mvvm.Model;
using DailyKanjiLogic.Mvvm.Model;
using System.Diagnostics;
using System.Windows.Navigation;

#nullable enable

namespace DailyKanji.Mvvm.View
{
    internal sealed partial class InfoWindow
    {
        #region Public Properties

        /// <summary>
        /// A data model that contain all data for the program logic and all Kanji data
        /// </summary>
        public MainBaseModel BaseModel { get; }

        /// <summary>
        /// A data model that contains all data for the surface and application
        /// </summary>
        public MainModel Model { get; }

        #endregion Public Properties

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoWindow"/> class
        /// </summary>
        /// <param name="baseModel">A data model that contain all data for the program logic and all Kanji data</param>
        /// <param name="model">A data model that contains all data for the surface and application</param>
        internal InfoWindow(MainBaseModel baseModel, MainModel model)
        {
            Model     = model;
            BaseModel = baseModel;

            InitializeComponent();
        }

        #endregion Internal Constructors

        #region Private Methods

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion Private Methods
    }
}
