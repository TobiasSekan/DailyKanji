using DailyKanji.Mvvm.ViewModel;
using System;

#nullable enable

namespace DailyKanji
{
    internal sealed partial class App
    {
        private void Application_Startup(object sender, EventArgs e)
            => new MainViewModel();
    }
}
