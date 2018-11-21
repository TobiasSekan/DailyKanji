using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
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

    // Next
    // ----
    // TODO: Add menu entry to hide answer shortcut hint
    // TODO: Add menu entry to hide visible timer(timeout is still running)
    // TODO: Add menu entry to deactivate timeout(hide visible timer too)
    // TODO: Make refresh interval for timer changeable via menu
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Add more menu entries to reset individual things of the statistics
    // TODO: Add main counter for each test (negative/positive)
    //       on right answers +1 on wrong answers - 1
    //       use this counter to calculate count of same tests
    //       use this count to order bottom test table
    // TODO: Add four options for hints
    //       "hints based on the current ask type"
    //       "hints always in Roomaji"
    //       "hints always in Hiragana"
    //       "hints always in Katakana"
    // TODO: Add menu underscores (for menu keyboard navigation)

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

    public sealed partial class MainViewModel
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

        public MainModel Model { get; private set; }

        public MainBaseModel BaseModel { get; private set; }

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

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
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
            BaseModel.AllTestsList = BaseModel.AllTestsList
                                              .OrderByDescending(found => found.WrongHiraganaCount + found.WrongKatakanaCount)
                                              .ThenByDescending(found => found.WrongHiraganaCount)
                                              .ThenByDescending(found => found.WrongKatakanaCount)
                                              .ThenByDescending(found => found.CorrectHiraganaCount + found.CorrectKatakanaCount)
                                              .ThenByDescending(found => found.CorrectHiraganaCount)
                                              .ThenByDescending(found => found.CorrectKatakanaCount).ToList();

            BuildTestPool();
            ChooseNewSign(GetRandomTest());
            ChooseNewPossibleAnswers();
            BuildAnswerMenuAndButtons();

            BaseModel.IgnoreInput   = false;
            BaseModel.TestStartTime = DateTime.UtcNow;

            Model.TestTimer.Start();
        }

        /// <summary>
        /// Build the test pool (wrong answered tests will add multiple)
        /// </summary>
        internal void BuildTestPool()
        {
            var testPool = new Collection<TestBaseModel>();

            foreach(var test in BaseModel.AllTestsList)
            {
                switch(BaseModel.SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                        for(var repeatCount = 0; repeatCount <= test.WrongHiraganaCount; repeatCount++)
                        {
                            testPool.Add(test);
                        }
                        break;

                    case TestType.KatakanaToHiragana:
                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                        for(var repeatCount = 0; repeatCount <= test.WrongKatakanaCount; repeatCount++)
                        {
                            testPool.Add(test);
                        }
                        break;

                    case TestType.HiraganaOrKatakanaToRoomaji:
                    case TestType.RoomajiToHiraganaOrKatakana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana:
                        for(var repeatCount = 0; repeatCount <= test.WrongHiraganaCount + test.WrongKatakanaCount; repeatCount++)
                        {
                            testPool.Add(test);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                }
            }

            BaseModel.TestPool = testPool;
        }

        /// <summary>
        /// Choose a new sign for a new ask
        /// </summary>
        internal void ChooseNewSign(TestBaseModel newTest)
        {
            if(BaseModel.CurrentTest != null)
            {
                while(newTest?.Roomaji == BaseModel.CurrentTest.Roomaji)
                {
                    newTest = GetRandomTest();
                }
            }

            BaseModel.CurrentTest = newTest;

            switch(BaseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                case TestType.HiraganaToKatakanaOrKatakanaOrHiragana:
                    BaseModel.CurrentAskSign = BaseModel.Randomizer.Next(0, 2) == 0
                        ? BaseModel.CurrentTest.Hiragana
                        : BaseModel.CurrentTest.Katakana;
                    break;

                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    BaseModel.CurrentAskSign = BaseModel.CurrentTest.Hiragana;
                    break;

                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    BaseModel.CurrentAskSign = BaseModel.CurrentTest.Katakana;
                    break;

                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                    BaseModel.CurrentAskSign = BaseModel.CurrentTest.Roomaji;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        internal void ChooseNewPossibleAnswers()
        {
            var list = new ObservableCollection<TestBaseModel>
            {
                BaseModel.CurrentTest
            };

            var tryAddCount = 0;

            while(list.Count < BaseModel.MaximumAnswers)
            {
                var possibleAnswer = GetRandomTest(onlyOneRoomajiCharacter: tryAddCount > 20);
                if(possibleAnswer == null)
                {
                    // TODO: investigate why "possibleAnswer" can be null
                    continue;
                }

                if(list.Any(found => found.Roomaji == possibleAnswer.Roomaji))
                {
                    continue;
                }

                if(!BaseModel.SimilarAnswers)
                {
                    list.Add(possibleAnswer);
                    continue;
                }

                if(tryAddCount < 50
                && !possibleAnswer.Roomaji.Contains(BaseModel.CurrentTest.Roomaji.FirstOrDefault())
                && !possibleAnswer.Roomaji.Contains(BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(1))
                && !possibleAnswer.Roomaji.Contains(BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(2)))
                {
                    tryAddCount++;
                    continue;
                }

                list.Add(possibleAnswer);
                tryAddCount = 0;
            }

            list.Shuffle();

            BaseModel.PossibleAnswers = list;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        internal void CheckAnswer(TestBaseModel answer)
        {
            if(BaseModel.IgnoreInput)
            {
                return;
            }

            BaseModel.IgnoreInput = true;

            Model.TestTimer.Stop();

            var answerTime = DateTime.UtcNow - BaseModel.TestStartTime;
            if(answer == null)
            {
                throw new ArgumentNullException(nameof(answer), "Test not found");
            }

            BaseModel.PreviousTest = BaseModel.CurrentTest;

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                // TODO: find a better way to check answer button text without use "_mainWindow" reference
                var stackPanels = _mainWindow.AnswerButtonArea.Children.OfType<StackPanel>();
                var childrens   = stackPanels.Select(found => found.Children);
                var buttons     = childrens.Select(found => found[1]).OfType<Button>();
                var contexts    = buttons.Select(found => found.Content);
                var textBlocks  = contexts.OfType<TextBlock>();
                var texts       = textBlocks.Select(found => found.Text);

                var isHiragana = texts.Any(found => found == BaseModel.CurrentTest.Hiragana);
                var isKatakana = texts.Any(found => found == BaseModel.CurrentTest.Katakana);

                if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
                {
                    switch(BaseModel.SelectedTestType)
                    {
                        case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                        case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when isKatakana:
                        case TestType.RoomajiToHiraganaOrKatakana when isHiragana:
                        case TestType.HiraganaToRoomaji:
                        case TestType.RoomajiToHiragana:
                        case TestType.HiraganaToKatakana:
                            BaseModel.CurrentTest.CompleteAnswerTimeForHiragana += answerTime;
                            BaseModel.CurrentTest.CorrectHiraganaCount++;
                            break;

                        case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                        case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when isHiragana:
                        case TestType.RoomajiToHiraganaOrKatakana when isKatakana:
                        case TestType.KatakanaToRoomaji:
                        case TestType.RoomajiToKatakana:
                        case TestType.KatakanaToHiragana:
                            BaseModel.CurrentTest.CompleteAnswerTimeForKatakana += answerTime;
                            BaseModel.CurrentTest.CorrectKatakanaCount++;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                    }

                    CreateNewTest();
                    return;
                }

                switch(BaseModel.SelectedTestType)
                {
                    case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when isKatakana:
                    case TestType.RoomajiToHiraganaOrKatakana when isHiragana:
                    case TestType.HiraganaToRoomaji:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakana:
                        BaseModel.CurrentTest.CompleteAnswerTimeForHiragana += answerTime;
                        BaseModel.CurrentTest.WrongHiraganaCount++;
                        break;

                    case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when isHiragana:
                    case TestType.RoomajiToHiraganaOrKatakana when isKatakana:
                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        BaseModel.CurrentTest.CompleteAnswerTimeForKatakana += answerTime;
                        BaseModel.CurrentTest.WrongKatakanaCount++;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                }

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

            for(var answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
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

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber] = new SolidColorBrush(Colors.Transparent);
                Model.HintTextColor[answerNumber]     = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Return a new random test
        /// </summary>
        /// <param name="onlyOneRoomajiCharacter">(Optional) Indicate that only a test that have a roomaji character with length one will return</param>
        /// <returns>A test</returns>
        internal TestBaseModel GetRandomTest(bool onlyOneRoomajiCharacter = false)
            => onlyOneRoomajiCharacter
                ? BaseModel.TestPool.Where(found => found.Roomaji.Length == 1)
                                       .ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count))
                : BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        internal void BuildAnswerMenuAndButtons()
            => _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerButtonArea.Children.Clear();
                _mainWindow.AnswerMenu.Items.Clear();

                for(var answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
                {
                    var text = string.Empty;
                    var hint = string.Empty;

                    switch(BaseModel.SelectedTestType)
                    {
                        case TestType.HiraganaOrKatakanaToRoomaji:
                        case TestType.HiraganaToRoomaji:
                        case TestType.KatakanaToRoomaji:
                            text = BaseModel.PossibleAnswers[answerNumber].Roomaji;
                            break;

                        case TestType.RoomajiToHiraganaOrKatakana:
                            text = BaseModel.Randomizer.Next(0, 2) == 0
                                    ? BaseModel.PossibleAnswers[answerNumber].Hiragana
                                    : BaseModel.PossibleAnswers[answerNumber].Katakana;
                            break;

                        case TestType.RoomajiToHiragana:
                        case TestType.KatakanaToHiragana:
                            text = BaseModel.PossibleAnswers[answerNumber].Hiragana;
                            break;

                        case TestType.RoomajiToKatakana:
                        case TestType.HiraganaToKatakana:
                            text = BaseModel.PossibleAnswers[answerNumber].Katakana;
                            break;

                        case TestType.HiraganaToKatakanaOrKatakanaOrHiragana:
                            text = BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana
                                    ? BaseModel.PossibleAnswers[answerNumber].Hiragana
                                    : BaseModel.PossibleAnswers[answerNumber].Katakana;
                            break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                    }

                    switch(BaseModel.SelectedTestType)
                    {
                        case TestType.RoomajiToHiraganaOrKatakana:
                        case TestType.RoomajiToHiragana:
                        case TestType.RoomajiToKatakana:
                            hint = BaseModel.PossibleAnswers[answerNumber].Roomaji;
                            break;

                        case TestType.HiraganaToRoomaji:
                        case TestType.HiraganaToKatakana:
                            hint = BaseModel.PossibleAnswers[answerNumber].Hiragana;
                            break;

                        case TestType.KatakanaToRoomaji:
                        case TestType.KatakanaToHiragana:
                            hint = BaseModel.PossibleAnswers[answerNumber].Katakana;
                            break;

                        case TestType.HiraganaOrKatakanaToRoomaji:
                        case TestType.HiraganaToKatakanaOrKatakanaOrHiragana:
                            hint = BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana
                                    ? BaseModel.PossibleAnswers[answerNumber].Hiragana
                                    : BaseModel.PossibleAnswers[answerNumber].Katakana;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                    }

                    var stackPanel = new StackPanel();
                    var buttonText = new TextBlock
                    {
                        FontSize          = 100 - (5 * BaseModel.MaximumAnswers),
                        Text              = text,
                        Padding           = new Thickness(0, 0, 0, 20),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    stackPanel.Children.Add(new TextBlock
                    {
                        FontSize            = 32,
                        Foreground          = Model.HintTextColor[answerNumber],
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text                = hint
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

                    stackPanel.Children.Add(new TextBlock
                    {
                        FontSize            = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text                = $"{answerNumber + 1}",
                    });

                    _mainWindow.AnswerButtonArea.Children.Add(stackPanel);

                    _mainWindow.AnswerMenu.Items.Add(new MenuItem
                    {
                        Command          = CommandAnswerTest,
                        CommandParameter = BaseModel.PossibleAnswers[answerNumber],
                        Header           = text,
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

        /// <summary>
        /// Rest the complete statistic
        /// </summary>
        internal void ResetCompleteStatistic()
        {
            foreach(var test in BaseModel.AllTestsList)
            {
                test.CorrectHiraganaCount          = 0;
                test.CorrectKatakanaCount          = 0;
                test.WrongHiraganaCount            = 0;
                test.WrongKatakanaCount            = 0;
                test.CompleteAnswerTimeForHiragana = new TimeSpan();
                test.CompleteAnswerTimeForKatakana = new TimeSpan();
            }
        }

        #endregion Internal Methods
    }
}
