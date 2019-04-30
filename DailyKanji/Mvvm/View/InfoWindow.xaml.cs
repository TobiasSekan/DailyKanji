using DailyKanji.Mvvm.ViewModel;
using System.Diagnostics;
using System.Windows.Navigation;

namespace DailyKanji.Mvvm.View
{
    internal sealed partial class InfoWindow
    {
        #region Public Properties

        public MainViewModel ViewModel { get; }

        #endregion Public Properties

        #region Internal Constructors

        internal InfoWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
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
