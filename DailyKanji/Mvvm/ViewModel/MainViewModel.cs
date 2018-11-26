using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using System;
using System.Collections.ObjectModel;
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

    // Next
    // ----
    // TODO: Add menu entry to deactivate timeout(hide visible timer too)
    // TODO: Make refresh interval for timer changeable via menu
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Add main counter for each test (negative/positive)
    //       on right answers +1 on wrong answers - 1
    //       use this counter to calculate count of same tests
    //       use this count to order bottom test table
    // TODO: Add menu underscores (for menu keyboard navigation)
    // TODO: Add German language and language selector in menu
    // TODO: Add message box with yes/no before delete statistics
    // TODO: Add option to deactivate error highlight

    // Near future
    // -----------
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

    // Later
    // -----
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Export (XLSX, CSV, JSON, XML)
    // TODO: Import (XLSX, CSV, JSON, XML)
    // TODO: Make colours choose-able
    // TODO: Gamepad support
    // TODO: Ribbon menu
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Check for new version on start-up
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    public sealed partial class MainViewModel : MainBaseViewModel
    {
        #region Private Properties

        private string _settingFileName
            => "settings.json";

        private Brush _progressBarColor
            => new SolidColorBrush(Colors.LightBlue);

        private Brush _errorColor
            => new SolidColorBrush(Colors.LightCoral);

        private Brush _correctColor
            => new SolidColorBrush(Colors.LightGreen);

        private MainWindow _mainWindow { get; }

        #endregion Private Properties

        #region Public Properties

        public MainModel Model { get; }

        #endregion Public Properties

        #region Public Constructors

        internal MainViewModel()
        {
            Model     = new MainModel();
            BaseModel = new MainBaseModel();

            LoadSettings();

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != BaseModel.AllTestsList?.Count())
            {
                BaseModel.AllTestsList = list.ToList();
            }

            BaseModel.Randomizer      = new Random();
            BaseModel.PossibleAnswers = new Collection<TestBaseModel>();
            BaseModel.TestPool        = new Collection<TestBaseModel>();

            Model.AnswerButtonColor = new ObservableCollection<Brush>();
            Model.HintTextColor     = new ObservableCollection<Brush>();
            Model.TestTimer         = new Timer { Interval = 15 };
            Model.ProgressBarColor  = new SolidColorBrush(Colors.LightBlue);

            Model.TestTimer.Elapsed += (_, __) =>
            {
                BaseModel.CurrentAnswerTime = (DateTime.UtcNow - BaseModel.TestStartTime).TotalMilliseconds;

                if(BaseModel.CurrentAnswerTime < BaseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                Model.TestTimer.Stop();
                CheckAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty));
            };

            for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor.Add(new SolidColorBrush(Colors.Transparent));
                Model.HintTextColor.Add(new SolidColorBrush(Colors.Transparent));
            }

            BuildTestPool();
            ChooseNewSign(GetRandomTest());

            _mainWindow = new MainWindow(this);

            CreateNewTest();
            RemoveAnswerColors();

            _mainWindow.Closed += (_, __) => SaveSettings();
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

            BaseModel.IgnoreInput   = false;
            BaseModel.TestStartTime = DateTime.UtcNow;

            Model.TestTimer.Start();
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        internal void CheckAnswer(in TestBaseModel answer)
        {
            if(BaseModel.IgnoreInput)
            {
                return;
            }

            BaseModel.IgnoreInput = true;

            Model.TestTimer.Stop();

            if(answer == null)
            {
                throw new ArgumentNullException(nameof(answer), "Test not found");
            }

            BaseModel.PreviousTest = BaseModel.CurrentTest;

            CountAnswerResult(answer);

            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
            {
                CreateNewTest();
                return;
            }

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                SetErrorColors();
                BuildAnswerMenuAndButtons();

                var timer = new Timer(BaseModel.ErrorTimeout)
                {
                    AutoReset = false
                };

                timer.Elapsed += (_, __) =>
                {
                    _mainWindow.Dispatcher.Invoke(new Action(() => RemoveAnswerColors()));
                    CreateNewTest();
                };

                timer.Start();
            }));
        }

        /// <summary>
        /// Set colours to all elements
        /// </summary>
        internal void SetErrorColors()
        {
            Model.CurrentAskSignColor = _errorColor;
            Model.ProgressBarColor    = _errorColor;

            for(byte answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber]
                    = BaseModel.PossibleAnswers[answerNumber].Roomaji == BaseModel.CurrentTest.Roomaji
                        ? _correctColor
                        : _errorColor;

                if(BaseModel.ShowHints)
                {
                    Model.HintTextColor[answerNumber] = new SolidColorBrush(Colors.Black);
                }
            }
        }

        /// <summary>
        /// Remove all colours form the answer buttons
        /// </summary>
        internal void RemoveAnswerColors()
        {
            Model.CurrentAskSignColor = new SolidColorBrush(Colors.Transparent);
            Model.ProgressBarColor    = _progressBarColor;

            for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber] = new SolidColorBrush(Colors.Transparent);
                Model.HintTextColor[answerNumber]     = new SolidColorBrush(Colors.Transparent);
            }
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
                        Foreground          = Model.HintTextColor[answerNumber],
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text                = GetAnswerHint(answerNumber)
                    });

                    stackPanel.Children.Add(new Button
                    {
                        Background       = Model.AnswerButtonColor[answerNumber],
                        Command          = CommandAnswerTest,
                        CommandParameter = BaseModel.PossibleAnswers[answerNumber],
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
                            Text                = $"{answerNumber + 1}",
                        });
                    }

                    _mainWindow.AnswerButtonArea.Children.Add(stackPanel);

                    _mainWindow.AnswerMenu.Items.Add(new MenuItem
                    {
                        Command          = CommandAnswerTest,
                        CommandParameter = BaseModel.PossibleAnswers[answerNumber],
                        Header           = GetAnswerText(answerNumber),
                        InputGestureText = $"{answerNumber + 1}"
                    });
                }
            }));

        /// <summary>
        /// Save all settings (data model) of this application
        /// </summary>
        internal void SaveSettings()
        {
            try
            {
                JsonHelper.WriteJson(_settingFileName, BaseModel);
            }
            catch(Exception exception)
            {
                MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{exception}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load all settings (data model) of this application
        /// </summary>
        internal void LoadSettings()
        {
            if(!File.Exists(_settingFileName))
            {
                return;
            }

            try
            {
                BaseModel = JsonHelper.ReadJson<MainBaseModel>(_settingFileName);
            }
            catch(Exception exception)
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{exception}",
                                $"Error on load {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        #endregion Internal Methods
    }
}
