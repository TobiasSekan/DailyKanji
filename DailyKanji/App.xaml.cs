using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.ViewModel;
using DailyKanjiLogic.Mvvm.ViewModel;
using System;
using System.IO;
using System.Windows;

#nullable enable

namespace DailyKanji
{
    internal sealed partial class App
    {
        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private static string _settingFileName
            => "settings.json";

        private void Application_Startup(object sender, EventArgs e)
        {
            if(!MainBaseViewModel.TryLoadSettings(_settingFileName, out var baseModel, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            var model         = new MainModel();
            var baseViewModel = new MainBaseViewModel(baseModel, ColorHelper.TransparentColor, ColorHelper.ProgressBarColor);

            new MainViewModel(model, baseModel, baseViewModel, _settingFileName);
        }
    }
}
