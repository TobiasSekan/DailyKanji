using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DailyKanji.Mvvm.ViewModel
{
    /// <summary>
    /// Partial class of the <see cref="MainViewModel"/> that contains the complete logic
    /// </summary>
    internal sealed partial class MainViewModel : IDisposable
    {
        #region Internal Properties

        /// <summary>
        /// The window contains all elements of the main window
        /// </summary>
        internal MainWindow? MainWindow { private get; set; }

        #endregion Internal Properties

        #region Private Properties

        /// <summary>
        /// A data model that contains all data for the surface and the application
        /// </summary>
        private MainModel Model { get; }

        /// <summary>
        /// A data model that contain all data for the program logic and all Kanji data
        /// </summary>
        private MainBaseModel BaseModel { get; }

        /// <summary>
        /// The view-model that contains the base program logic
        /// </summary>
        private MainBaseViewModel BaseViewModel { get; }

        #endregion Private Properties

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class
        /// </summary>
        /// <param name="baseModel"> A data model that contain all data for the program logic and all Kanji data</param>
        /// <param name="model">A data model that contains all data for the surface and the application</param>
        /// <param name="baseViewModel"></param>
        internal MainViewModel(MainBaseModel baseModel, MainModel model, MainBaseViewModel baseViewModel)
        {
            Model            = model;
            BaseModel        = baseModel;
            BaseViewModel    = baseViewModel;

            Model.TestTimer.Elapsed += (_, __) =>
            {
                BaseModel.AnswerTime = DateTime.UtcNow - BaseModel.TestStartTime;

                if(!BaseModel.UseAnswerTimer
                || BaseModel.AnswerTime < BaseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                Model.TestTimer.Stop();
                CheckSelectedAnswer(TestBaseModel.EmptyTest);
            };

            CheckForNewVersion();
            GamepadTest();

            BaseViewModel.PrepareNewTest();
            BaseViewModel.SetNormalColors();
        }

        #endregion Internal Constructors

        #region IDisposable Implementation

        /// <inheritdoc/>
        public void Dispose()
            => BaseModel.Dispose();

        #endregion IDisposable Implementation

        #region Internal Methods

        /// <summary>
        /// Do all work to show and start a new test
        /// </summary>
        internal void ShowAndStartNewTest()
        {
            BuildAnswerMenuAndButtons();

            BaseModel.OnPropertyChangeForAll();

            RestartTestTimer();

            BaseModel.IgnoreInput = false;
        }

        /// <summary>
        /// Restart the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void RestartTestTimer()
        {
            Model.TestTimer.Stop();

            BaseModel.TestStartTime = DateTime.UtcNow;

            if(!BaseModel.UseAnswerTimer)
            {
                return;
            }

            BaseModel.ProgressBarColor = ColorHelper.ProgressBarColor;

            Model.TestTimer.Start();
        }

        /// <summary>
        /// Move and resize the <see cref="MainWindow"/>, based on the values inside the <see cref="BaseModel"/>
        /// </summary>
        internal void MoveAndResizeWindowToLastPosition()
        {
            if(MainWindow is null)
            {
                return;
            }

            if(!double.IsNaN(BaseModel.WindowHigh))
            {
                MainWindow.Height = BaseModel.WindowHigh;
            }

            if(!double.IsNaN(BaseModel.WindowWidth))
            {
                MainWindow.Width = BaseModel.WindowWidth;
            }

            if(!double.IsNaN(BaseModel.LeftPosition))
            {
                MainWindow.Left = BaseModel.LeftPosition;
            }

            if(double.IsNaN(BaseModel.TopPosition))
            {
                return;
            }

            MainWindow.Top = BaseModel.TopPosition;
        }

        /// <summary>
        /// Set the size and the position values inside the <see cref="BaseModel"/>, based on the values of the <see cref="MainWindow"/>
        /// </summary>
        internal void SetWindowSizeAndPositionInTheMainModel()
        {
            if(MainWindow == null)
            {
                return;
            }

            BaseModel.WindowHigh   = MainWindow.Height;
            BaseModel.WindowWidth  = MainWindow.Width;
            BaseModel.LeftPosition = MainWindow.Left;
            BaseModel.TopPosition  = MainWindow.Top;
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        internal void BuildAnswerMenuAndButtons()
        {
            Model.AnswerHintTextHeight     = BaseModel.SelectedHintShowType != HintShowType.None ? GridLength.Auto : new GridLength(0);
            Model.AnswerShortCutTextHeight = BaseModel.ShowAnswerShortcuts ? GridLength.Auto : new GridLength(0);

            var answerMenu                   = new List<MenuItem>(10);
            var markMenu                     = new List<MenuItem>(10);
            var answerButtonColumnWidth      = new List<GridLength>(10);
            var answerButtonVisibility       = new List<Visibility>(10);
            var answerAnswerText             = new List<string>(10);
            var answerHintText               = new List<string>(10);
            var answerShortCutText           = new List<string>(10);
            var answersType                  = BaseViewModel.GetAnswerType();

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                if(answerNumber >= BaseModel.MaximumAnswers)
                {
                    answerButtonColumnWidth.Add(GridLength.Auto);
                    answerButtonVisibility.Add(Visibility.Collapsed);
                    answerAnswerText.Add(string.Empty);
                    answerHintText.Add(string.Empty);
                    answerShortCutText.Add(string.Empty);

                    continue;
                }

                var answer           = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                var answerText       = MainBaseViewModel.GetAnswerText(answer, answersType);
                var inputGestureText = answerNumber < 9 ? $"{answerNumber + 1}" : "0";

                answerButtonColumnWidth.Add(new GridLength(1, GridUnitType.Star));
                answerButtonVisibility.Add(Visibility.Visible);
                answerAnswerText.Add(answerText);
                answerHintText.Add(BaseViewModel.GetAnswerHint(answer));
                answerShortCutText.Add(BaseModel.ShowAnswerShortcuts ? inputGestureText : string.Empty);
            }

            MainWindow?.Dispatcher.Invoke(() =>
            {
                for(var answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber >= BaseModel.MaximumAnswers)
                    {
                        continue;
                    }

                    var answer           = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                    var answerText       = MainBaseViewModel.GetAnswerText(answer, answersType);
                    var inputGestureText = answerNumber < 9 ? $"{answerNumber + 1}" : "0";

                    answerMenu.Add(new MenuItem
                    {
                        Command          = new CommandHelper(value => CheckSelectedAnswer(value as TestBaseModel ?? TestBaseModel.EmptyTest)),
                        CommandParameter = answer,
                        Header           = answerText,
                        InputGestureText = inputGestureText,
                    });

                    markMenu.Add(new MenuItem
                    {
                        Command          = new CommandHelper(value => HighlightAnswer(value as TestBaseModel ?? TestBaseModel.EmptyTest)),
                        CommandParameter = answer,
                        Header           = answerText,
                        InputGestureText = $"Shift+{inputGestureText}",
                    });
                }

                Model.AnswerMenu                   = answerMenu;
                Model.MarkMenu                     = markMenu;
                Model.AnswerButtonColumnWidth      = answerButtonColumnWidth;
                Model.AnswerButtonVisibility       = answerButtonVisibility;
                Model.AnswerAnswerText             = answerAnswerText;
                Model.AnswerHintText               = answerHintText;
                Model.AnswerShortCutText           = answerShortCutText;
            });
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        private void CheckSelectedAnswer(in TestBaseModel answer)
        {
            Debug.Assert(!(answer is null), "Answer can't be null for check selected answer");

            if(BaseModel.IgnoreInput || answer is null)
            {
                return;
            }

            Model.TestTimer.Stop();

            BaseModel.AnswerTime = DateTime.UtcNow - BaseModel.TestStartTime;

            var test   = (TestBaseModel)answer.Clone();
            var result = BaseViewModel.CheckAndCountAnswer(answer);

            BaseViewModel.RefreshAndSetHighlightForStatisticValues(test);

            if((result && !BaseModel.HighlightOnCorrectAnswer) || (!result && !BaseModel.HighlightOnWrongAnswer))
            {
                BaseViewModel.PrepareNewTest();
                ShowAndStartNewTest();
                return;
            }

            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            _ = Task.Run(() =>
            {
                if(BaseModel.HighlightTimer.SafeWaitHandle.IsClosed)
                {
                    return;
                }

                BaseViewModel.SetHighlightColors(answerTemp);
                BaseViewModel.PrepareNewTest();

                _ = BaseModel.HighlightTimer.WaitOne(BaseModel.HighlightTimeout);

                BaseViewModel.ResetHighlight();
                BaseViewModel.SetNormalColors();

                ShowAndStartNewTest();
            });
        }

        /// <summary>
        /// Check if a new version on-line
        /// </summary>
        private void CheckForNewVersion()
        {
            if(!BaseModel.CheckForNewVersionOnStartUp)
            {
                return;
            }

            try
            {
                var yourVersion   = AssemblyHelper.GetAssemblyVersion(this);
                var onLineVersion = OnlineResourceHelper.GetVersion(
                                        "https://raw.githubusercontent.com/TobiasSekan/DailyKanji/master/DailyKanji/Properties/AssemblyInfo.cs");

                if(yourVersion.Equals(onLineVersion))
                {
                    return;
                }

                if(MessageBox.Show(
                        $"A new version of Daily Kanji is available.{Environment.NewLine}{Environment.NewLine}"
                        + $"Your version:\t{yourVersion}{Environment.NewLine}"
                        + $"On-line version:\t{onLineVersion}{Environment.NewLine}{Environment.NewLine}"
                        + "Do you want to go to website to download it?",
                        "Version check",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information)
                   != MessageBoxResult.Yes)
                {
                    return;
                }

                _ = Process.Start("https://github.com/TobiasSekan/DailyKanji/releases");
            }
            catch(Exception exception)
            {
                _ = MessageBox.Show(
                        $"Can't check for updates{Environment.NewLine}{Environment.NewLine}{exception}",
                        "Error on check for updates",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Test if a joystick or game-pad is connected to this computer
        /// </summary>
        private void GamepadTest()
        {
            // TODO:
            // - Add "joystick.Unacquire();" on program close
            // - Move more game-pad logic to DailyKanjiLogic project
            // - Refresh "maxButtonCount" on answer change
            // - Check for disconnected joystick/game-pad
            // - Show button names as hints, when game-pad is connected
            // - Show joystick state and button count inside the status bar

            var gamepad = DirectInputHelper.GetFirstGamePad();
            if(gamepad is null)
            {
                return;
            }

            var maxButtonCount = Math.Min(BaseModel.MaximumAnswers, gamepad.Capabilities.ButtonCount);

            _ = Task.Run(() =>
            {
                using var manualResetEvent = new ManualResetEvent(false);

                while(true)
                {
                    _ = manualResetEvent.WaitOne(100);

                    var gamepadButtonState = gamepad.GetCurrentState();
                    if(gamepadButtonState is null)
                    {
                        continue;
                    }

                    for(var button = 0; button < maxButtonCount; button++)
                    {
                        if(!gamepadButtonState.Buttons.ElementAtOrDefault(button))
                        {
                            continue;
                        }

                        // TODO: check if "button + 1" is the correct value
                        CheckSelectedAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(button + 1));
                    }
                }
            });
        }

        /// <summary>
        /// Highlight a answer (button) with the <see cref="_noneSelectedColor"/>
        /// </summary>
        /// <param name="answer">The answer (button) to highlight</param>
        private void HighlightAnswer(in TestBaseModel answer)
        {
            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            MainWindow?.Dispatcher.Invoke(() => BaseViewModel.SetOrRemoveHighlightColorToOneAnswer(answerTemp));
        }

        #endregion Private Methods
    }
}
