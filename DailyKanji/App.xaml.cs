using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanji.Mvvm.ViewModel;
using DailyKanjiLogic.Mvvm.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace DailyKanji
{
    internal sealed partial class App
    {
        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private static string _settingsFileName
            => "settings.json";

        private void Application_Startup(object sender, EventArgs e)
        {
            // TODO: move this calls back to the "mainViewModel" constructor
            if(!MainBaseViewModel.TryLoadSettings(_settingsFileName, out var baseModel, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {_settingsFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            var model         = new MainModel();
            var baseViewModel = new MainBaseViewModel(baseModel);
            var mainViewModel = new MainViewModel(baseModel, model, baseViewModel);
            var mainWindow    = new MainWindow(baseModel, model, mainViewModel);

            // TODO: The "mainViewModel" should not contain the view model
            mainViewModel.MainWindow = mainWindow;

            // TODO: move this calls back to the "mainViewModel" constructor
            mainViewModel.ShowAndStartNewTest();
            mainViewModel.MoveAndResizeWindowToLastPosition();

            mainWindow.Closed += (_, __) =>
            {
                mainViewModel.SetWindowSizeAndPositionInTheMainModel();

                if(!baseViewModel.TrySaveSettings(_settingsFileName, out var saveException))
                {
                    MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{saveException}",
                                    $"Error on save {_settingsFileName}",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

                mainViewModel.Dispose();
                model.Dispose();
            };

            mainWindow.Show();
        }
    }
}
