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

        public MainBaseModel BaseModel { get; }

        public MainModel Model { get; }

        #endregion Public Properties

        #region Internal Constructors

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
