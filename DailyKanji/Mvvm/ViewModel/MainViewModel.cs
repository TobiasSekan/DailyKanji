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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DailyKanji.Mvvm.ViewModel
{
    // BUG
    // ---
    // TODO: Bug inside command helper, so predicate is not usable on commands
    // TODO: Fix correct counting for wrong answers on test type "RoomajiToHiraganaOrKatakana"

    // Version 0.1
    // -----------
    // TODO: Add menu entry to deactivate timeout(hide visible timer too)
    // TODO: Make refresh interval for timer changeable via menu
    // TODO: Add main counter for each test (negative/positive)
    //       on right answers +1 on wrong answers - 1
    //       use this counter to calculate count of same tests
    //       use this count to order bottom test table

    // Version 1.0
    // -----------
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Recalculate buttons (button width), when window is resized
    // TODO: Avoid rebuild of answer buttons and answer menu entries
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
    // TODO: Move more program parts to separate library project in .Net Standard 1.0
    // TODO: Export (CSV, JSON)
    // TODO: Add more menu underscores (for menu keyboard navigation)
    // TODO: Add tooltips for each menu entries

    // Version 2.0
    // -----------
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Add German language and language selector in menu
    // TODO: Make colours choose-able

    // Version 3.0
    // -----------
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Export (XLSX, XML)
    // TODO: Import (XLSX, CSV, JSON, XML)
    // TODO: Gamepad support
    // TODO: Ribbon menu
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    public sealed partial class MainViewModel : MainBaseViewModel
    {
        #region Private Properties

        private string _settingFileName
            => "settings.json";

        private string _progressBarColor
            => Colors.LightBlue.ToString();

        private string _errorColor
            => Colors.LightCoral.ToString();

        private string _correctColor
            => Colors.LightGreen.ToString();

        private string _transparentColor
            => Colors.Transparent.ToString();

        private string _hintColor
            => Colors.Black.ToString();

        private MainWindow _mainWindow { get; }

        #endregion Private Properties

        #region Public Properties

        public MainModel Model { get; }

        #endregion Public Properties

        #region Public Constructors

        internal MainViewModel()
        {
            if(!TryLoadSettings(_settingFileName, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            Model = new MainModel
            {
                TestTimer           = new Timer(15),
                ErrorHighlightTimer = new Timer(BaseModel.ErrorTimeout)
                {
                    AutoReset = false
                }
            };

            InitalizieBaseModel(_transparentColor, _progressBarColor);

            Model.TestTimer.Elapsed += (_, __) =>
            {
                BaseModel.CurrentAnswerTime = (DateTime.UtcNow - BaseModel.TestStartTime).TotalMilliseconds;

                if(BaseModel.CurrentAnswerTime < BaseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                Model.TestTimer.Stop();
                CheckSelectedAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty));
            };

            Model.ErrorHighlightTimer.Elapsed += (_, __) =>
            {
                Model.ErrorHighlightTimer.Stop();
                _mainWindow.Dispatcher.Invoke(new Action(() => SetNormalColors(_transparentColor, _progressBarColor)));
                CreateNewTest();
                return;
            };

            _mainWindow = new MainWindow(this);

            CheckForNewVersion();

            CreateNewTest();
            SetNormalColors(_transparentColor, _progressBarColor);

            _mainWindow.Closed += (_, __) =>
            {
                if(TrySaveSettings(_settingFileName, out var saveException))
                {
                    return;
                }

                MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{saveException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            };

            _mainWindow.Show();
        }

        #endregion Public Constructors

        #region Internal Methods

        /// <summary>
        /// Create a new test with new question and new possible answers
        /// </summary>
        internal void CreateNewTest()
        {
            OrderAllTests();
            BuildTestPool();
            ChooseNewSign(GetRandomTest());
            ChooseNewPossibleAnswers();
            BuildAnswerMenuAndButtons();
            StartTestTimer();

            BaseModel.IgnoreInput   = false;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal void CheckSelectedAnswer(in TestBaseModel answer)
        {
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

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                SetHighlightColors(_correctColor, _errorColor, _hintColor);
                BuildAnswerMenuAndButtons();
                Model.ErrorHighlightTimer.Start();
            }));
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        internal void BuildAnswerMenuAndButtons()
            => _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerButtonArea.Children.Clear();
                _mainWindow.AnswerMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
                {
                    var hintColor   = ColorConverter.ConvertFromString(BaseModel.HintTextColor
                                                                                .ElementAtOrDefault(answerNumber));

                    var answerColor = ColorConverter.ConvertFromString(BaseModel.AnswerButtonColor
                                                                                .ElementAtOrDefault(answerNumber));

                    var stackPanel = new StackPanel();
                    var buttonText = new TextBlock
                    {
                        FontSize          = 100 - (5 * BaseModel.MaximumAnswers),
                        Text              = GetAnswerText(answerNumber),
                        Padding           = new Thickness(0, 0, 0, 20),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    stackPanel.Children.Add(new TextBlock
                    {
                        FontSize            = 32,
                        Foreground          = new SolidColorBrush((Color)hintColor),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text                = GetAnswerHint(answerNumber)
                    });

                    stackPanel.Children.Add(new Button
                    {
                        Background       = new SolidColorBrush((Color)answerColor),
                        Command          = CommandAnswerTest,
                        CommandParameter = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber),
                        Content          = buttonText,
                        Height           = 100,
                        Margin           = new Thickness(5, 0, 5, 0),
                        Width            = (_mainWindow.Width - 20 - (10 * BaseModel.MaximumAnswers)) / BaseModel.MaximumAnswers
                    });

                    if(BaseModel.ShowAnswerShortcuts)
                    {
                        stackPanel.Children.Add(new TextBlock
                        {
                            FontSize            = 12,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Text                = $"{answerNumber + 1}"
                        });
                    }

                    _mainWindow.AnswerButtonArea.Children.Add(stackPanel);

                    _mainWindow.AnswerMenu.Items.Add(new MenuItem
                    {
                        Command          = CommandAnswerTest,
                        CommandParameter = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber),
                        Header           = GetAnswerText(answerNumber),
                        InputGestureText = $"{answerNumber + 1}"
                    });
                }
            }));

        /// <summary>
        /// Start the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void StartTestTimer()
        {
            BaseModel.TestStartTime = DateTime.UtcNow;
            Model.TestTimer.Start();
        }

        internal void CheckForNewVersion()
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
                                   MessageBoxImage.Information) != MessageBoxResult.Yes)
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

        #endregion Internal Methods
    }
}
