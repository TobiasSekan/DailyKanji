using System;
using DailyKanji.Mvvm.ViewModel;

namespace DailyKanji
{
    public partial class App
    {
        private void Application_Startup(object sender, EventArgs e)
            => new MainViewModel();
    }
}
