﻿using DailyKanji.Helper;
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
    // Current sign statistics (possible: show wrong count)

    // BUG
    // ---
    // Fix correct counting for wrong answers on test type "RoomajiToHiraganaOrKatakana"

    // Version 1.x
    // -----------
    // TODO: Show highlight and tool-tip on right answers (activate via option)
    // TODO: Show wrong and not selected answers in yellow
    // TODO: Game-pad support (with 10 buttons for 10 answers)
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers
    //
    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers
    //
    // TODO: Add similar list for each Hiragana and each Katakana character for option "Similar answers"
    // TODO: Change test order so that all tests will be ask (based on ask counter)
    // TODO: Add more menu underscores (for menu keyboard navigation)
    // TODO: Make refresh interval for timer changeable via menu

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
        /// The colour string for the progress bar
        /// </summary>
        private static string ProgressBarColor
            => Colors.LightBlue.ToString();

        /// <summary>
        /// The colour string for the error highlight
        /// </summary>
        private static string ErrorColor
            => Colors.LightCoral.ToString();

        /// <summary>
        /// The colour string for the correct answer (on error highlight)
        /// </summary>
        private static string CorrectColor
            => Colors.LightGreen.ToString();

        /// <summary>
        /// The colour string for invisible text and elements
        /// </summary>
        private static string TransparentColor
            => Colors.Transparent.ToString();

        /// <summary>
        /// The colour string for the answer hints (on error highlight)
        /// </summary>
        private static string HintColor
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

            CreateNewTest();
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
        /// Create a new test with new question and new possible answers
        /// </summary>
        private void CreateNewTest()
        {
            OrderAllTests();
            BuildTestPool();
            ChooseNewSign(GetRandomKanaTest());
            ChooseNewPossibleAnswers();
            BuildAnswerMenuAndButtons();
            StartTestTimer();

            BaseModel.IgnoreInput = false;
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

            if(CheckAnswer(answer))
            {
                CreateNewTest();
                return;
            }

            if(!BaseModel.HighlightOnErrors)
            {
                CreateNewTest();
                return;
            }

            MainWindow.Dispatcher.Invoke(() =>
            {
                SetHighlightColors(CorrectColor, ErrorColor, HintColor);
                BuildAnswerMenuAndButtons();

                Task.Run(() =>
                {
                    BaseModel.ErrorHighlightTimer.WaitOne(BaseModel.ErrorTimeout);

                    MainWindow.Dispatcher.Invoke(() => SetNormalColors(TransparentColor, ProgressBarColor));
                    CreateNewTest();
                });
            });
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        private void BuildAnswerMenuAndButtons()
            => MainWindow?.Dispatcher?.Invoke(() =>
            {
                MainWindow.AnswerMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber < BaseModel.MaximumAnswers)
                    {
                        MainWindow.AnswerButtonColumn[answerNumber].Width = new GridLength(1, GridUnitType.Star);

                        MainWindow.ButtonList[answerNumber].Visibility              = Visibility.Visible;
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Visibility = Visibility.Visible;
                        MainWindow.AnswerHintTextBlock[answerNumber].Visibility     = Visibility.Visible;

                        MainWindow.AnswerTextList[answerNumber].Text      = GetAnswerText(answerNumber);
                        MainWindow.AnswerHintTextBlock[answerNumber].Text = GetAnswerHint(answerNumber);
                        MainWindow.AnswerShortCutTextBlock[answerNumber].Text = BaseModel.ShowAnswerShortcuts
                                ? $"{answerNumber + 1}"
                                : string.Empty;

                        MainWindow.AnswerMenu.Items.Add(new MenuItem
                        {
                            Command          = new CommandHelper(value => CheckSelectedAnswer(value as TestBaseModel)),
                            CommandParameter = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber),
                            Header           = GetAnswerText(answerNumber),
                            InputGestureText = $"{answerNumber + 1}"
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

        /// <summary>
        /// Start the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        private void StartTestTimer()
        {
            if(!BaseModel.UseAnswerTimer)
            {
                return;
            }

            BaseModel.TestStartTime = DateTime.UtcNow;
            Model.TestTimer.Start();
        }

        /// <summary>
        /// Check if a new version online
        /// </summary>
        private void CheckForNewVersion()
        {
            if(!BaseModel.CheckForNewVersionOnStartUp)
            {
                return;
            }

            try
            {
                var yourVersion = AssemblyHelper.GetAssemblyVersion(this);

                var onlineVersion = OnlineResourceHelper.GetVersion(
                                        "https://raw.githubusercontent.com/TobiasSekan/DailyKanji/master/DailyKanji/Properties/AssemblyInfo.cs");

                if(yourVersion.Equals(onlineVersion))
                {
                    return;
                }

                if(MessageBox.Show($"A new version of Daily Kanji is available.{Environment.NewLine}{Environment.NewLine}"
                                   + $"Your version:\t{yourVersion}{Environment.NewLine}"
                                   + $"Online version:\t{onlineVersion}{Environment.NewLine}{Environment.NewLine}"
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

            var gamepad = DirectInputHelper.GetFirstGamepad();
            if(gamepad is null)
            {
                return;
            }

            var maxButtonCount   = Math.Min(BaseModel.MaximumAnswers, gamepad.Capabilities.ButtonCount);
            var manualResetEvent = new ManualResetEvent(false);

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

                        CheckSelectedAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(button + 1));
                    }
                }
            });
        }

        #endregion Internal Methods
    }
}
