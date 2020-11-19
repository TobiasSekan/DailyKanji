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
        #region Private Properties

        /// <summary>
        /// A data model that contains all data for the surface and the application
        /// </summary>
        private MainModel _model { get; }

        /// <summary>
        /// A data model that contain all data for the program logic and all Kanji data
        /// </summary>
        private MainBaseModel _baseModel { get; }

        /// <summary>
        /// The view-model that contains the base program logic
        /// </summary>
        private MainBaseViewModel _baseViewModel { get; }

        /// <summary>
        /// The window contains all elements of the main window
        /// </summary>
        internal MainWindow? MainWindow { private get; set; }

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
            _model            = model;
            _baseModel        = baseModel;
            _baseViewModel    = baseViewModel;

            _model.TestTimer.Elapsed += (_, __) =>
            {
                _baseModel.AnswerTime = DateTime.UtcNow - _baseModel.TestStartTime;

                if(!_baseModel.UseAnswerTimer
                || _baseModel.AnswerTime < _baseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                _model.TestTimer.Stop();
                CheckSelectedAnswer(TestBaseModel.EmptyTest);
            };

            CheckForNewVersion();
            GamepadTest();

            _baseViewModel.PrepareNewTest();
            _baseViewModel.SetNormalColors();
        }

        #endregion Internal Constructors

        #region IDisposable Implementation

        /// <inheritdoc/>
        public void Dispose()
            => _baseModel.Dispose();

        #endregion IDisposable Implementation

        #region Internal Methods

        /// <summary>
        /// Do all work to show and start a new test
        /// </summary>
        internal void ShowAndStartNewTest()
        {
            BuildAnswerMenuAndButtons();

            _baseModel.OnPropertyChangeForAll();

            RestartTestTimer();

            _baseModel.IgnoreInput = false;
        }

        /// <summary>
        /// Restart the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void RestartTestTimer()
        {
            _model.TestTimer.Stop();

            _baseModel.TestStartTime = DateTime.UtcNow;

            if(!_baseModel.UseAnswerTimer)
            {
                return;
            }

            _baseModel.ProgressBarColor = ColorHelper.ProgressBarColor;

            _model.TestTimer.Start();
        }

        /// <summary>
        /// Move and resize the <see cref="MainWindow"/>, based on the values inside the <see cref="_baseModel"/>
        /// </summary>
        internal void MoveAndResizeWindowToLastPosition()
        {
            if(MainWindow is null)
            {
                return;
            }

            if(!double.IsNaN(_baseModel.WindowHigh))
            {
                MainWindow.Height = _baseModel.WindowHigh;
            }

            if(!double.IsNaN(_baseModel.WindowWidth))
            {
                MainWindow.Width = _baseModel.WindowWidth;
            }

            if(!double.IsNaN(_baseModel.LeftPosition))
            {
                MainWindow.Left = _baseModel.LeftPosition;
            }

            if(double.IsNaN(_baseModel.TopPosition))
            {
                return;
            }

            MainWindow.Top = _baseModel.TopPosition;
        }

        /// <summary>
        /// Set the size and the position values inside the <see cref="_baseModel"/>, based on the values of the <see cref="MainWindow"/>
        /// </summary>
        internal void SetWindowSizeAndPositionInTheMainModel()
        {
            if(MainWindow == null)
            {
                return;
            }

            _baseModel.WindowHigh   = MainWindow.Height;
            _baseModel.WindowWidth  = MainWindow.Width;
            _baseModel.LeftPosition = MainWindow.Left;
            _baseModel.TopPosition  = MainWindow.Top;
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        internal void BuildAnswerMenuAndButtons()
        {
            _model.AnswerHintTextHeight     = _baseModel.SelectedHintShowType != HintShowType.ShowOnNoAnswers ? GridLength.Auto : new GridLength(0);
            _model.AnswerShortCutTextHeight = _baseModel.ShowAnswerShortcuts ? GridLength.Auto : new GridLength(0);

            var answerMenu                   = new List<MenuItem>(10);
            var markMenu                     = new List<MenuItem>(10);
            var answerButtonColumnWidth      = new List<GridLength>(10);
            var answerButtonVisibility       = new List<Visibility>(10);
            var answerAnswerText             = new List<string>(10);
            var answerHintText               = new List<string>(10);
            var answerShortCutText           = new List<string>(10);
            var answersType                  = _baseViewModel.GetAnswerType();

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                if(answerNumber >= _baseModel.MaximumAnswers)
                {
                    answerButtonColumnWidth.Add(GridLength.Auto);
                    answerButtonVisibility.Add(Visibility.Collapsed);
                    answerAnswerText.Add(string.Empty);
                    answerHintText.Add(string.Empty);
                    answerShortCutText.Add(string.Empty);

                    continue;
                }

                var answer           = _baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                var answerText       = MainBaseViewModel.GetAnswerText(answer, answersType);
                var inputGestureText = answerNumber < 9 ? $"{(answerNumber + 1).ToString()}" : "0";

                answerButtonColumnWidth.Add(new GridLength(1, GridUnitType.Star));
                answerButtonVisibility.Add(Visibility.Visible);
                answerAnswerText.Add(answerText);
                answerHintText.Add(_baseViewModel.GetAnswerHint(answer));
                answerShortCutText.Add(_baseModel.ShowAnswerShortcuts ? inputGestureText : string.Empty);
            }

            MainWindow?.Dispatcher.Invoke(() =>
            {
                for(var answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber >= _baseModel.MaximumAnswers)
                    {
                        continue;
                    }

                    var answer           = _baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                    var answerText       = MainBaseViewModel.GetAnswerText(answer, answersType);
                    var inputGestureText = answerNumber < 9 ? $"{(answerNumber + 1).ToString()}" : "0";

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

                _model.AnswerMenu                   = answerMenu;
                _model.MarkMenu                     = markMenu;
                _model.AnswerButtonColumnWidth      = answerButtonColumnWidth;
                _model.AnswerButtonVisibility       = answerButtonVisibility;
                _model.AnswerAnswerText             = answerAnswerText;
                _model.AnswerHintText               = answerHintText;
                _model.AnswerShortCutText           = answerShortCutText;
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

            if(_baseModel.IgnoreInput || answer is null)
            {
                return;
            }

            _model.TestTimer.Stop();

            _baseModel.AnswerTime = DateTime.UtcNow - _baseModel.TestStartTime;

            var test   = (TestBaseModel)answer.Clone();
            var result = _baseViewModel.CheckAndCountAnswer(answer);

            _baseViewModel.RefreshAndSetHighlightForStatisticValues(test);

            if((result && !_baseModel.HighlightOnCorrectAnswer) || (!result && !_baseModel.HighlightOnWrongAnswer))
            {
                _baseViewModel.PrepareNewTest();
                ShowAndStartNewTest();
                return;
            }

            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            Task.Run(() =>
            {
                if(_baseModel.HighlightTimer.SafeWaitHandle.IsClosed)
                {
                    return;
                }

                _baseViewModel.SetHighlightColors(answerTemp);
                _baseViewModel.PrepareNewTest();

                _baseModel.HighlightTimer.WaitOne(_baseModel.HighlightTimeout);

                _baseViewModel.ResetHighlight();
                _baseViewModel.SetNormalColors();

                ShowAndStartNewTest();
            });
        }

        /// <summary>
        /// Check if a new version on-line
        /// </summary>
        private void CheckForNewVersion()
        {
            if(!_baseModel.CheckForNewVersionOnStartUp)
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

                if(MessageBox.Show($"A new version of Daily Kanji is available.{Environment.NewLine}{Environment.NewLine}"
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

                Process.Start("https://github.com/TobiasSekan/DailyKanji/releases");
            }
            catch(Exception exception)
            {
                MessageBox.Show($"Can't check for updates{Environment.NewLine}{Environment.NewLine}{exception}",
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

            var maxButtonCount = Math.Min(_baseModel.MaximumAnswers, gamepad.Capabilities.ButtonCount);

            Task.Run(() =>
            {
                using var manualResetEvent = new ManualResetEvent(false);

                while(true)
                {
                    manualResetEvent.WaitOne(100);

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
                        CheckSelectedAnswer(_baseModel.PossibleAnswers.ElementAtOrDefault(button + 1));
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

            MainWindow?.Dispatcher.Invoke(() => _baseViewModel.SetOrRemoveHighlightColorToOneAnswer(answerTemp));
        }

        #endregion Private Methods
    }
}
