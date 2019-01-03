using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
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
    // TODO: Fix size problem with less then four answers (option: 3 answers, option: 2 answers)
    // TODO: Fix crash on low and empty test pool (lower 10 entries)

    // Version 1.0
    // -----------
    // TODO: Make it possible to activate only one kana type
    // TODO: Add test for sign with Yooon, Yooon with Dakuten and Yooon with Handakuten
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

    // Version 2.0
    // -----------
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Add German language and language selector in menu
    // TODO: Add tooltips for each menu entries
    // TODO: Make colours choose-able
    // TODO: Export statistics (CSV, JSON, XML)

    // Version 3.0
    // -----------
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Export statistics (XLSX)
    // TODO: Import statistics (XLSX, CSV, JSON, XML)
    // TODO: Gamepad support
    // TODO: Ribbon menu
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    public sealed partial class MainViewModel : MainBaseViewModel
    {
        #region Private Properties

        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private string _settingFileName
            => "settings.json";

        /// <summary>
        /// The colour string for the progress bar
        /// </summary>
        private string _progressBarColor
            => Colors.LightBlue.ToString();

        /// <summary>
        /// The colour string for the error highlight
        /// </summary>
        private string _errorColor
            => Colors.LightCoral.ToString();

        /// <summary>
        /// The colour string for the correct answer (on error highlight)
        /// </summary>
        private string _correctColor
            => Colors.LightGreen.ToString();

        /// <summary>
        /// The colour string for invisible text and elements
        /// </summary>
        private string _transparentColor
            => Colors.Transparent.ToString();

        /// <summary>
        /// The colour string for the answer hints (on error highlight)
        /// </summary>
        private string _hintColor
            => Colors.Black.ToString();

        /// <summary>
        /// The main viewable window of this application
        /// </summary>
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
                CheckSelectedAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty, KanaType.Gojuuon));
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
        {
            _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber < BaseModel.MaximumAnswers)
                    {
                        _mainWindow._answerButtonColumn[answerNumber].Width           = new GridLength(1, GridUnitType.Star);

                        _mainWindow._buttonList[answerNumber].Visibility              = Visibility.Visible;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Visibility = Visibility.Visible;
                        _mainWindow._answerHintTextBlock[answerNumber].Visibility     = Visibility.Visible;

                        _mainWindow._answerTextList[answerNumber].Text                = GetAnswerText(answerNumber);
                        _mainWindow._answerHintTextBlock[answerNumber].Text           = GetAnswerHint(answerNumber);
                        _mainWindow._answerShortCutTextBlock[answerNumber].Text       = BaseModel.ShowAnswerShortcuts
                                                                                            ? $"{answerNumber + 1}"
                                                                                            : string.Empty;

                        _mainWindow.AnswerMenu.Items.Add(new MenuItem
                        {
                            Command          = CommandAnswerTest,
                            CommandParameter = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber),
                            Header           = GetAnswerText(answerNumber),
                            InputGestureText = $"{answerNumber + 1}"
                        });
                    }
                    else
                    {
                        _mainWindow._answerButtonColumn[answerNumber].Width           = GridLength.Auto;

                        _mainWindow._buttonList[answerNumber].Visibility              = Visibility.Collapsed;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Visibility = Visibility.Collapsed;
                        _mainWindow._answerHintTextBlock[answerNumber].Visibility     = Visibility.Collapsed;

                        _mainWindow._answerTextList[answerNumber].Text                = string.Empty;
                        _mainWindow._answerHintTextBlock[answerNumber].Text           = string.Empty;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Text       = string.Empty;
                    }
                }
            }));
        }

        /// <summary>
        /// Start the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void StartTestTimer()
        {
            if(!BaseModel.UseAnswerTimer)
            {
                return;
            }

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
