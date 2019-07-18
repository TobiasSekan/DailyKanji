using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

#nullable enable

namespace DailyKanji.Mvvm.ViewModel
{
    // Test
    // ----
    // Test: Current sign statistics (possible: show wrong count)
    // Test: Correct counting for answers on test type "AllToAll"
    // Test: Game-pad button calculation
    // Test: Game-pad support (with 10 buttons for 10 answers)

    // BUG
    // ---

    // Version 1.x
    // -----------
    // TODO: Show up or down indicator for wrong count, correct count and average answer time
    // TODO: Refresh sign statistics (wrong count, correct count and average answer time) in main window direct after answer
    // TODO  Add UnitTests - NUnit with Assert.That()
    // TODO: Add extended Katakana(see https://en.wikipedia.org/wiki/Transcription_into_Japanese#Extended_katakana_2)
    // TODO: Add German language and language selector in menu
    // TODO: Add tool-tips for each menu entries
    // TODO: Add more menu underscores (for menu keyboard navigation)
    // TODO: Add similar list for each Hiragana and each Katakana character for option "Similar answers"
    // TODO: Change test order so that all tests will be ask (based on ask counter)
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers
    //
    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers

    // Version 2.x
    // -----------
    // Internal: DailyKanjiLogic.Mvvm.ViewModel.GetAnswerNumber -> Can we use foreach here ?
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Move more program parts to separate library project in .Net Standard
    // TODO: Export statistics (XLSX, CSV, JSON, XML)
    // TODO: Import statistics (XLSX, CSV, JSON, XML)
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Make colors choose-able
    // TODO: Ribbon menu

    // Version 3.x
    // -----------
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    internal sealed partial class MainViewModel
    {
        #region Internal Properties

        private MainModel _model { get; }

        private MainBaseModel _baseModel { get; }

        private MainBaseViewModel _baseViewModel { get; }

        #endregion Internal Properties

        #region Private Properties

        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private static string _settingFileName
            => "settings.json";

        /// <summary>
        /// The main viewable window of this application
        /// </summary>
        private MainWindow _mainWindow { get; }

        #endregion Private Properties

        #region Internal Constructors

        internal MainViewModel()
        {
            if(!MainBaseViewModel.TryLoadSettings(_settingFileName, out var baseModel, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            _model         = new MainModel();
            _baseModel     = baseModel;
            _baseViewModel = new MainBaseViewModel(baseModel, ColorHelper.TransparentColor, ColorHelper.ProgressBarColor);

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

            _mainWindow = new MainWindow(_baseModel, _model, this);

            CheckForNewVersion();

            _baseViewModel.PrepareNewTest();
            ShowAndStartNewTest();
            _baseViewModel.SetNormalColors(ColorHelper.TransparentColor, ColorHelper.ProgressBarColor);

            _mainWindow.Closed += (_, __) =>
            {
                SetWindowSizeAndPositionInTheMainModel();

                if(_baseViewModel.TrySaveSettings(_settingFileName, out var saveException))
                {
                    return;
                }

                MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{saveException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            };

            GamepadTest();

            _mainWindow.Show();

            MoveAndResizeWindowToLastPosition();
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// Restart the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void RestartTestTimer()
        {
            _model.TestTimer.Stop();

            if(!_baseModel.UseAnswerTimer)
            {
                return;
            }

            _baseModel.ProgressBarColor = ColorHelper.ProgressBarColor;
            _baseModel.TestStartTime    = DateTime.UtcNow;

            _model.TestTimer.Start();
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Do all work to show and start a new test
        /// </summary>
        private void ShowAndStartNewTest()
        {
            BuildAnswerMenuAndButtons();

            _baseModel.OnPropertyChangeForAll();

            RestartTestTimer();

            _baseModel.IgnoreInput = false;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void CheckSelectedAnswer(in TestBaseModel answer)
        {
            Debug.Assert(!(answer is null), "Answer can't be null for check selected answer");

            if(_baseModel.IgnoreInput)
            {
                return;
            }

            _model.TestTimer.Stop();

            var correctCounterBefore = _baseModel.CurrentTest.CorrectHiraganaCount         + _baseModel.CurrentTest.CorrectKatakanaCount;
            var wrongCounterBefore   = _baseModel.CurrentTest.WrongHiraganaCount           + _baseModel.CurrentTest.WrongKatakanaCount;
            var answerTimeBefore     = _baseModel.CurrentTest.AverageAnswerTimeForHiragana + _baseModel.CurrentTest.AverageAnswerTimeForKatakana;

            var result = _baseViewModel.CheckAndCountAnswer(answer);
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
                _mainWindow.Dispatcher.Invoke(() => SetHighlight(correctCounterBefore, wrongCounterBefore, answerTimeBefore, result, answerTemp));

                _baseViewModel.PrepareNewTest();

                _baseModel.HighlightTimer.WaitOne(_baseModel.HighlightTimeout);

                _mainWindow.Dispatcher.Invoke(RemoveHighlight);

                ShowAndStartNewTest();
            });
        }

        /// <summary>
        /// Set highlight on all elements on the main window
        /// </summary>
        /// <param name="correctCounterBefore">The correct count before the answer</param>
        /// <param name="wrongCounterBefore">The wrong count before the answer</param>
        /// <param name="answerTimeBefore">The answer time before the answer</param>
        /// <param name="result">The result of the answer</param>
        /// <param name="answerTemp">A local copy of the answer</param>
        private void SetHighlight(uint correctCounterBefore, uint wrongCounterBefore, TimeSpan answerTimeBefore, bool result, TestBaseModel answerTemp)
        {
            _model.HighlightCorrectCounter =
                correctCounterBefore != (_baseModel.CurrentTest.CorrectHiraganaCount + _baseModel.CurrentTest.CorrectKatakanaCount);

            _model.HighlightWrongCounter =
                wrongCounterBefore != (_baseModel.CurrentTest.WrongHiraganaCount + _baseModel.CurrentTest.WrongKatakanaCount);

            _model.HighlightAnswerTime =
                answerTimeBefore != (_baseModel.CurrentTest.AverageAnswerTimeForHiragana + _baseModel.CurrentTest.AverageAnswerTimeForKatakana);

            _baseViewModel.SetHighlightColors(answerTemp,
                                              ColorHelper.CorrectColor,
                                              result ? ColorHelper.CorrectColor : ColorHelper.ErrorColor,
                                              ColorHelper.NoneSelectedColor, ColorHelper.AnswerHintTextColor);
        }

        /// <summary>
        /// Remove highlight of all elements on the main window
        /// </summary>
        private void RemoveHighlight()
        {
            _model.HighlightCorrectCounter = false;
            _model.HighlightWrongCounter   = false;
            _model.HighlightAnswerTime     = false;

            _baseViewModel.SetNormalColors(ColorHelper.TransparentColor, ColorHelper.ProgressBarColor);
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        private void BuildAnswerMenuAndButtons()
        {
            var answersType = _baseViewModel.GetAnswerType();

            _mainWindow?.Dispatcher?.Invoke(() =>
            {
                _mainWindow.AnswerMenu.Items.Clear();
                _mainWindow.MarkMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber < _baseModel.MaximumAnswers)
                    {
                        var answer           = _baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                        var answerText       = MainBaseViewModel.GetAnswerText(answer, answersType);
                        var inputGestureText = answerNumber < 9 ? $"{answerNumber + 1}" : "0";

                        _mainWindow.AnswerButtonColumn[answerNumber].Width = new GridLength(1, GridUnitType.Star);

                        _mainWindow.ButtonList[answerNumber].Visibility              = Visibility.Visible;
                        _mainWindow.AnswerShortCutTextBlock[answerNumber].Visibility = Visibility.Visible;
                        _mainWindow.AnswerHintTextBlock[answerNumber].Visibility     = Visibility.Visible;

                        _mainWindow.AnswerTextList[answerNumber].Text          = answerText;
                        _mainWindow.AnswerHintTextBlock[answerNumber].Text     = _baseViewModel.GetAnswerHint(answer);
                        _mainWindow.AnswerShortCutTextBlock[answerNumber].Text = _baseModel.ShowAnswerShortcuts
                                ? inputGestureText
                                : string.Empty;

                        _mainWindow.AnswerMenu.Items.Add(new MenuItem
                        {
                            Command          = new CommandHelper(value => CheckSelectedAnswer(value as TestBaseModel ?? TestBaseModel.EmptyTest)),
                            CommandParameter = answer,
                            Header           = answerText,
                            InputGestureText = inputGestureText
                        });

                        _mainWindow.MarkMenu.Items.Add(new MenuItem
                        {
                            Command          = new CommandHelper(value => HighlightAnswer(value as TestBaseModel ?? TestBaseModel.EmptyTest)),
                            CommandParameter = answer,
                            Header           = answerText,
                            InputGestureText = $"Shift+{inputGestureText}"
                        });
                    }
                    else
                    {
                        _mainWindow.AnswerButtonColumn[answerNumber].Width = GridLength.Auto;

                        _mainWindow.ButtonList[answerNumber].Visibility              = Visibility.Collapsed;
                        _mainWindow.AnswerShortCutTextBlock[answerNumber].Visibility = Visibility.Collapsed;
                        _mainWindow.AnswerHintTextBlock[answerNumber].Visibility     = Visibility.Collapsed;

                        _mainWindow.AnswerTextList[answerNumber].Text          = string.Empty;
                        _mainWindow.AnswerHintTextBlock[answerNumber].Text     = string.Empty;
                        _mainWindow.AnswerShortCutTextBlock[answerNumber].Text = string.Empty;
                    }
                }
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

        private void GamepadTest()
        {
            // TODO:
            // - Add "joystick.Unacquire();" on program close
            // - Move more gamepad logic to DailyKanjiLogic project
            // - Refresh "maxButtonCount" on answer change
            // - Check for disconnected joystick/gamepad
            // - Show button names as hints, when gamepad is connected
            // - Show joystick state and button count inside the status bar

            var gamepad = DirectInputHelper.GetFirstGamePad();
            if(gamepad is null)
            {
                return;
            }

            var maxButtonCount   = Math.Min(_baseModel.MaximumAnswers, gamepad.Capabilities.ButtonCount);

            using(var manualResetEvent = new ManualResetEvent(false))
            {
                Task.Run(() =>
                {
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
        }

        /// <summary>
        /// Highlight a answer (button) with the <see cref="_noneSelectedColor"/>
        /// </summary>
        /// <param name="answer">The answer (button) to highlight</param>
        private void HighlightAnswer(in TestBaseModel answer)
        {
            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            _mainWindow.Dispatcher.Invoke(() => _baseViewModel.SetOrRemoveHighlightColorToOneAnswer(answerTemp,
                                                                                                    ColorHelper.NoneSelectedColor,
                                                                                                    ColorHelper.TransparentColor));
        }

        /// <summary>
        /// Move and resize the <see cref="_mainWindow"/>, based on the values inside the <see cref="_baseModel"/>
        /// </summary>
        private void MoveAndResizeWindowToLastPosition()
            {
            if(!double.IsNaN(_baseModel.WindowHigh))
            {
                _mainWindow.Height = _baseModel.WindowHigh;
            }

            if(!double.IsNaN(_baseModel.WindowWidth))
            {
                _mainWindow.Width = _baseModel.WindowWidth;
            }

            if(!double.IsNaN(_baseModel.LeftPosition))
            {
                _mainWindow.Left = _baseModel.LeftPosition;
            }

            if(double.IsNaN(_baseModel.TopPosition))
            {
                return;
            }

            _mainWindow.Top = _baseModel.TopPosition;
        }

        /// <summary>
        /// Set the size and the position values inside the <see cref="_baseModel"/>, based on the values of the <see cref="_mainWindow"/>
        /// </summary>
        private void SetWindowSizeAndPositionInTheMainModel()
        {
            _baseModel.WindowHigh   = _mainWindow.Height;
            _baseModel.WindowWidth  = _mainWindow.Width;
            _baseModel.LeftPosition = _mainWindow.Left;
            _baseModel.TopPosition  = _mainWindow.Top;
        }

        #endregion Private Methods
    }
}
