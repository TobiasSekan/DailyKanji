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
using System.Windows.Media;

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
    // BUG: Test pool have the wrong count of signs
    // BUG: Answers type flip on highlight time on test type "AllToAll"
    // BUG: Partial crash when no answer is selected

    // Version 1.x
    // -----------
    // TODO: Make refresh interval for timer changeable via menu
    // TODO: Add similar list for each Hiragana and each Katakana character for option "Similar answers"
    //
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers
    //
    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers
    //
    // TODO: Change test order so that all tests will be ask (based on ask counter)
    // TODO: Add more menu underscores (for menu keyboard navigation)

    // Version 2.x
    // -----------
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Add German language and language selector in menu
    // TODO: Add tool-tips for each menu entries
    // TODO: Make colors choose-able
    // TODO: Export statistics (XLSX, CSV, JSON, XML)

    // Version 3.x
    // -----------
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Import statistics (XLSX, CSV, JSON, XML)
    // TODO: Ribbon menu
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    internal sealed partial class MainViewModel : MainBaseViewModel
    {
        #region Public Properties

        public MainModel Model { get; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private static string SettingFileName
            => "settings.json";

        /// <summary>
        /// The color string for the progress bar (<see cref="Colors.LightBlue"/> - #FFADD8E6)
        /// </summary>
        private static string ProgressBarColor
            => Colors.LightBlue.ToString();

        /// <summary>
        /// The color string for the error highlight (<see cref="Colors.LightCoral"/> - #FFF08080)
        /// </summary>
        private static string ErrorColor
            => Colors.LightCoral.ToString();

        /// <summary>
        /// The color string for none selected answers (<see cref="Colors.LightGoldenrodYellow"/> - #FFFAFAD2)
        /// </summary>
        private static string NoneSelectedColor
            => Colors.LightGoldenrodYellow.ToString();

        /// <summary>
        /// The color string for the correct answer (<see cref="Colors.LightGreen"/> - FF90EE90)
        /// </summary>
        private static string CorrectColor
            => Colors.LightGreen.ToString();

        /// <summary>
        /// The color string for invisible text and elements (<see cref="Colors.Transparent"/> - #00FFFFFF)
        /// </summary>
        private static string TransparentColor
            => Colors.Transparent.ToString();

        /// <summary>
        /// The color string for the answer hints (<see cref="Colors.Black"/> - #FF000000)
        /// </summary>
        private static string AnswerHintTextColor
            => Colors.Black.ToString();

        /// <summary>
        /// The main viewable window of this application
        /// </summary>
        private MainWindow MainWindow { get; }

        #endregion Private Properties

        #region Internal Constructors

        internal MainViewModel()
        {
            if(!TryLoadSettings(SettingFileName, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {SettingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            Model = new MainModel
            {
                TestTimer = new System.Timers.Timer(15)
            };

            InitalizeBaseModel(TransparentColor, ProgressBarColor);

            Model.TestTimer.Elapsed += (_, __) =>
            {
                if(!BaseModel.UseAnswerTimer)
                {
                    return;
                }

                BaseModel.CurrentAnswerTime = (DateTime.UtcNow - BaseModel.TestStartTime).TotalMilliseconds;

                if(BaseModel.CurrentAnswerTime < BaseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                Model.TestTimer.Stop();
                CheckSelectedAnswer(TestBaseModel.EmptyTest);
            };

            MainWindow = new MainWindow(this);

            CheckForNewVersion();

            CreateNewTest(PrepareNewTest());
            SetNormalColors(TransparentColor, ProgressBarColor);

            MainWindow.Closed += (_, __) =>
            {
                if(TrySaveSettings(SettingFileName, out var saveException))
                {
                    return;
                }

                MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{saveException}",
                                $"Error on save {SettingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            };

            GamepadTest();

            MainWindow.Show();
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// Restart the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void RestartTestTimer()
        {
            Model.TestTimer.Stop();

            if(!BaseModel.UseAnswerTimer)
            {
                return;
            }

            BaseModel.ProgressBarColor = ProgressBarColor;
            BaseModel.TestStartTime    = DateTime.UtcNow;

            Model.TestTimer.Start();
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Create a new test with new question and new possible answers
        /// </summary>
        private void CreateNewTest(TestBaseModel test)
        {
            ChooseNewSign(test);
            ChooseNewPossibleAnswers();
            BuildAnswerMenuAndButtons();
            RestartTestTimer();

            BaseModel.IgnoreInput = false;
        }

        /// <summary>
        /// Do background work for a new test and return it (no surface changes on main window)
        /// </summary>
        /// <returns>A new test</returns>
        private TestBaseModel PrepareNewTest()
        {
            OrderAllTests();
            BuildTestPool();

            return GetRandomKanaTest();
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void CheckSelectedAnswer(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, "Answer can't be null for check selected answer");

            if(BaseModel.IgnoreInput)
            {
                return;
            }

            Model.TestTimer.Stop();

            var result = CheckAndCountAnswer(answer);

            if((result && !BaseModel.HighlightOnCorrectAnswer) || (!result && !BaseModel.HighlightOnWrongAnswer))
            {
                CreateNewTest(PrepareNewTest());
                return;
            }

            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            MainWindow.Dispatcher.Invoke(() =>
            {
                SetHighlightColors(answerTemp, CorrectColor, result ? CorrectColor : ErrorColor, NoneSelectedColor, AnswerHintTextColor);
                BuildAnswerMenuAndButtons();

                var newTest = PrepareNewTest();

                Task.Run(() =>
                {
                    BaseModel.HighlightTimer.WaitOne(BaseModel.HighlightTimeout);

                    MainWindow.Dispatcher.Invoke(() => SetNormalColors(TransparentColor, ProgressBarColor));
                    CreateNewTest(newTest);
                });
            });
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        private void BuildAnswerMenuAndButtons()
        {
            var answersType = GetAnswerType();

            MainWindow?.Dispatcher?.Invoke(() =>
            {
                MainWindow.AnswerMenu.Items.Clear();
                MainWindow.MarkMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber < BaseModel.MaximumAnswers)
                    {
                        var answer           = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                        var answerText       = GetAnswerText(answer, answersType);
                        var inputGestureText = answerNumber < 9 ? $"{answerNumber + 1}" : "0";

                        MainWindow.AnswerButtonColumn[answerNumber].Width = new GridLength(1, GridUnitType.Star);

                        MainWindow.ButtonList[answerNumber].Visibility              = Visibility.Visible;
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Visibility = Visibility.Visible;
                        MainWindow.AnswerHintTextBlock[answerNumber].Visibility     = Visibility.Visible;

                        MainWindow.AnswerTextList[answerNumber].Text          = answerText;
                        MainWindow.AnswerHintTextBlock[answerNumber].Text     = GetAnswerHint(answer);
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Text = BaseModel.ShowAnswerShortcuts
                                ? inputGestureText
                                : string.Empty;

                        MainWindow.AnswerMenu.Items.Add(new MenuItem
                        {
                            Command          = new CommandHelper(value => CheckSelectedAnswer(value as TestBaseModel)),
                            CommandParameter = answer,
                            Header           = answerText,
                            InputGestureText = inputGestureText
                        });

                        MainWindow.MarkMenu.Items.Add(new MenuItem
                        {
                            Command          = new CommandHelper(value => HighlightAnswer(value as TestBaseModel)),
                            CommandParameter = answer,
                            Header           = answerText,
                            InputGestureText = $"Shift+{inputGestureText}"
                        });
                    }
                    else
                    {
                        MainWindow.AnswerButtonColumn[answerNumber].Width = GridLength.Auto;

                        MainWindow.ButtonList[answerNumber].Visibility              = Visibility.Collapsed;
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Visibility = Visibility.Collapsed;
                        MainWindow.AnswerHintTextBlock[answerNumber].Visibility     = Visibility.Collapsed;

                        MainWindow.AnswerTextList[answerNumber].Text          = string.Empty;
                        MainWindow.AnswerHintTextBlock[answerNumber].Text     = string.Empty;
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Text = string.Empty;
                    }
                }
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

            var maxButtonCount   = Math.Min(BaseModel.MaximumAnswers, gamepad.Capabilities.ButtonCount);

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
                            CheckSelectedAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(button + 1));
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Highlight a answer (button) with the <see cref="NoneSelectedColor"/>
        /// </summary>
        /// <param name="answer">The answer (button) to highlight</param>
        private void HighlightAnswer(in TestBaseModel answer)
        {
            // can't use "in" parameter in anonymous method
            var answerTemp = answer;

            MainWindow.Dispatcher.Invoke(() => SetOrRemoveHighlightColorToOneAnswer(answerTemp, NoneSelectedColor, TransparentColor));
        }

        #endregion Private Methods
    }
}
