using DailyKanji.Enumerations;
using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DailyKanji.Mvvm.ViewModel
{
    // Next
    // ----
    // TODO: Show Roomaji on wrong answer test of type "Hiragana to Katakana" and "Katakana to Hiragana"
    // TODO: Show Hiragana on wrong answer test of type "Katakana to Roomaji" and "Roomaji to Katakana"
    // TODO: Show Katakana on wrong answer test of type "Hiragana to Roomaji" and "Roomaji to Hiragana"
    // TODO: Add test type for "Hiragana or Katakana to Katakana or Hiragana"
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Add menu entry to reset the statistics
    // TODO: Add new answers sub-menu (show current answer inside menu entry with shortcut)
    // TODO: Recalculate buttons (button width), when window is resized
    // TODO: Visible timer in 0.1 second (can be deactivated via menu)
    // TODO: Add main counter for each test (negative/positive)
    //       on right answers +1 on wrong answers - 1
    //       use this counter to calculate count of same tests
    //       use this count to order bottom test table

    // Near future
    // -----------
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers
    //
    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers
    //
    // TODO: Move logic to separate library project in .Net Standard 2.0
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Export (XLSX, CSV, JSON, XML)

    // Later
    // -----
    // TODO: Change test order so that all tests will be ask (based on ask counter)
    // TODO: Make colours choose-able
    // TODO: Import ???

    public sealed partial class MainViewModel
    {
        #region Private Properties

        private string _settingFileName => "settings.json";

        private MainWindow _mainWindow { get; }

        #endregion Private Properties

        #region Public Properties

        public MainModel Model { get; private set; }

        #endregion Public Properties

        #region Public Constructors

        internal MainViewModel()
        {
            Model = new MainModel();

            LoadSettings();

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != Model.AllTestsList?.Count())
            {
                Model.AllTestsList = list.ToList();
            }

            Model.Randomizer        = new Random();
            Model.AnswerButtonColor = new ObservableCollection<Brush>();
            Model.PossibleAnswers   = new ObservableCollection<TestBaseModel>();
            Model.NewQuestionList   = new Collection<TestBaseModel>();

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor.Add(new SolidColorBrush(Colors.Transparent));
            }

            _mainWindow = new MainWindow(this);

            BuildNewQuestionList();
            ChooseNewSign();
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
            Model.AllTestsList = Model.AllTestsList
                                      .OrderByDescending(found => found.WrongHiraganaCount + found.WrongKatakanaCount)
                                      .ThenByDescending(found => found.WrongHiraganaCount)
                                      .ThenByDescending(found => found.WrongKatakanaCount)
                                      .ThenByDescending(found => found.CorrectHiraganaCount + found.CorrectKatakanaCount)
                                      .ThenByDescending(found => found.CorrectHiraganaCount)
                                      .ThenByDescending(found => found.CorrectKatakanaCount).ToList();

            BuildNewQuestionList();
            ChooseNewSign();
            ChooseNewPossibleAnswers();
            BuildAnswerButtons();

            Model.IgnoreInput   = false;
            Model.TestStartTime = DateTime.UtcNow;
        }

        internal void BuildNewQuestionList()
        {
            var questionList = new Collection<TestBaseModel>();

            // TODO: refactor this

            foreach(var question in Model.AllTestsList)
            {
                if(Model.MainTestType == TestType.HiraganaToKatakana)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongHiraganaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.HiraganaToKatakana;
                        questionList.Add(question);
                    }
                }

                if(Model.MainTestType == TestType.KatakanaToHiragana)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongKatakanaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.KatakanaToHiragana;
                        questionList.Add(question);
                    }
                }

                if(Model.MainTestType == TestType.HiraganaToRoomaji || Model.MainTestType == TestType.HiraganaOrKatakanaToRoomaji)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongHiraganaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.HiraganaToRoomaji;
                        questionList.Add(question);
                    }
                }

                if(Model.MainTestType == TestType.KatakanaToRoomaji || Model.MainTestType == TestType.HiraganaOrKatakanaToRoomaji)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongKatakanaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.KatakanaToRoomaji;
                        questionList.Add(question);
                    }
                }

                if(Model.MainTestType == TestType.RoomajiToHiragana || Model.MainTestType == TestType.RoomajiToHiraganaOrKatakana)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongHiraganaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.RoomajiToHiragana;
                        questionList.Add(question);
                    }
                }

                if(Model.MainTestType == TestType.RoomajiToKatakana || Model.MainTestType == TestType.RoomajiToHiraganaOrKatakana)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongKatakanaCount + 1; repeatCount++)
                    {
                        question.TestType = TestType.RoomajiToKatakana;
                        questionList.Add(question);
                    }
                }
            }

            Model.NewQuestionList = questionList;
        }

        /// <summary>
        /// Choose a new sign for a new ask
        /// </summary>
        internal void ChooseNewSign()
        {
            var newQuest = GetRandomTest();

            if(Model.CurrentTest != null)
            {
                while(newQuest?.Roomaji == Model.CurrentTest.Roomaji)
                {
                    newQuest = GetRandomTest();
                }
            }

            Model.CurrentTest = newQuest;

            switch(Model.CurrentTest.TestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                    Model.CurrentAskSign = Model.Randomizer.Next(0, 2) == 0
                        ? Model.CurrentTest.Hiragana
                        : Model.CurrentTest.Katakana;
                    break;

                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    Model.CurrentAskSign = Model.CurrentTest.Hiragana;
                    break;

                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    Model.CurrentAskSign = Model.CurrentTest.Katakana;
                    break;

                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                    Model.CurrentAskSign = Model.CurrentTest.Roomaji;
                    break;
            }
        }

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        internal void ChooseNewPossibleAnswers()
        {
            var list = new ObservableCollection<TestBaseModel>
            {
                Model.CurrentTest
            };

            var tryAddCount = 0;

            while(list.Count < Model.MaximumAnswer)
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

                if(!Model.SimilarAnswers)
                {
                    list.Add(possibleAnswer);
                    continue;
                }

                if(tryAddCount < 50
                && !possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.FirstOrDefault())
                && !possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.ElementAtOrDefault(1))
                && !possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.ElementAtOrDefault(2)))
                {
                    tryAddCount++;
                    continue;
                }

                list.Add(possibleAnswer);
                tryAddCount = 0;
            }

            list.Shuffle();

            Model.PossibleAnswers = list;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        internal void CheckAnswer(TestBaseModel answer)
        {
            if(Model.IgnoreInput)
            {
                return;
            }

            Model.IgnoreInput = true;

            var answerTime = DateTime.UtcNow - Model.TestStartTime;

            if(answer == null)
            {
                throw new ArgumentNullException(nameof(answer), "Test not found");
            }

            // TODO: find a better way to check answer button text without use "_mainWindow" reference
            var answerList = _mainWindow.AnswerButtonArea.Children.OfType<StackPanel>()
                                                                  .Select(found => found.Children[0])
                                                                  .OfType<Button>()
                                                                  .Select(found => found.Content)
                                                                  .OfType<TextBlock>()
                                                                  .Select(found => found.Text);

            var isHiragana = answerList.Any(found => found == Model.CurrentTest.Hiragana);
            var isKatakana = answerList.Any(found => found == Model.CurrentTest.Katakana);

            if(answer.Roomaji == Model.CurrentTest.Roomaji)
            {
                switch(Model.MainTestType)
                {
                    case TestType.HiraganaOrKatakanaToRoomaji when Model.CurrentAskSign == Model.CurrentTest.Hiragana:
                    case TestType.RoomajiToHiraganaOrKatakana when isHiragana:
                    case TestType.HiraganaToRoomaji:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakana:
                        Model.CurrentTest.CompleteAnswerTimeForHiragana += answerTime;
                        Model.CurrentTest.CorrectHiraganaCount++;
                        break;

                    case TestType.HiraganaOrKatakanaToRoomaji when Model.CurrentAskSign == Model.CurrentTest.Katakana:
                    case TestType.RoomajiToHiraganaOrKatakana when isKatakana:
                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        Model.CurrentTest.CompleteAnswerTimeForKatakana += answerTime;
                        Model.CurrentTest.CorrectKatakanaCount++;
                        break;
                }

                CreateNewTest();
                return;
            }

            switch(Model.MainTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji when Model.CurrentAskSign == Model.CurrentTest.Hiragana:
                case TestType.RoomajiToHiraganaOrKatakana when isHiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.RoomajiToHiragana:
                case TestType.HiraganaToKatakana:
                    Model.CurrentTest.CompleteAnswerTimeForHiragana += answerTime;
                    Model.CurrentTest.WrongHiraganaCount++;
                    break;

                case TestType.HiraganaOrKatakanaToRoomaji when Model.CurrentAskSign == Model.CurrentTest.Katakana:
                case TestType.RoomajiToHiraganaOrKatakana when isKatakana:
                case TestType.KatakanaToRoomaji:
                case TestType.RoomajiToKatakana:
                case TestType.KatakanaToHiragana:
                    Model.CurrentTest.CompleteAnswerTimeForKatakana += answerTime;
                    Model.CurrentTest.WrongKatakanaCount++;
                    break;
            }

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                SetAnswerColors();
                BuildAnswerButtons();
            }));

            var timer = new System.Timers.Timer(Model.ErrorTimeout)
            {
                AutoReset = false
            };

            timer.Elapsed += (_, __) =>
            {
                _mainWindow.Dispatcher.Invoke(new Action(() => RemoveAnswerColors()));
                CreateNewTest();
            };

            timer.Start();
        }

        /// <summary>
        /// Set colours to all answer buttons (based on the test)
        /// </summary>
        internal void SetAnswerColors()
        {
            Model.CurrentAskSignColor = new SolidColorBrush(Colors.LightCoral);

            for(var answerNumber = 0; answerNumber < Model.MaximumAnswer; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber]
                    = new SolidColorBrush(Model.PossibleAnswers[answerNumber].Roomaji == Model.CurrentTest.Roomaji
                                            ? Colors.LightGreen
                                            : Colors.LightCoral);
            }
        }

        /// <summary>
        /// Remove all colours form the answer buttons
        /// </summary>
        internal void RemoveAnswerColors()
        {
            Model.CurrentAskSignColor = new SolidColorBrush(Colors.Transparent);

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber] = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Return a new random test
        /// </summary>
        /// <param name="onlyOneRoomajiCharacter">(Optional) Indicate that only a test that have a roomaji character with length one will return</param>
        /// <returns>A test</returns>
        internal TestBaseModel GetRandomTest(bool onlyOneRoomajiCharacter = false)
            => onlyOneRoomajiCharacter
                ? Model.NewQuestionList.Where(found => found.Roomaji.Length == 1)
                                       .ElementAtOrDefault(Model.Randomizer.Next(0, Model.NewQuestionList.Count))
                : Model.NewQuestionList.ElementAtOrDefault(Model.Randomizer.Next(0, Model.NewQuestionList.Count));

        /// <summary>
        /// Build all answer buttons (with text and colours)
        /// </summary>
        internal void BuildAnswerButtons()
            => _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerButtonArea.Children.Clear();

                for(var answerNumber = 0; answerNumber < Model.MaximumAnswer; answerNumber++)
                {
                    var text = string.Empty;

                    switch(Model.MainTestType)
                    {
                        case TestType.HiraganaOrKatakanaToRoomaji:
                        case TestType.HiraganaToRoomaji:
                        case TestType.KatakanaToRoomaji:
                            text = Model.PossibleAnswers[answerNumber].Roomaji;
                            break;

                        case TestType.RoomajiToHiraganaOrKatakana:
                            text = Model.Randomizer.Next(0, 2) == 0
                                    ? Model.PossibleAnswers[answerNumber].Hiragana
                                    : Model.PossibleAnswers[answerNumber].Katakana;
                            break;

                        case TestType.RoomajiToHiragana:
                        case TestType.KatakanaToHiragana:
                            text = Model.PossibleAnswers[answerNumber].Hiragana;
                            break;

                        case TestType.RoomajiToKatakana:
                        case TestType.HiraganaToKatakana:
                            text = Model.PossibleAnswers[answerNumber].Katakana;
                            break;
                    }

                    var stackPanel = new StackPanel();

                    var buttonText = new TextBlock
                    {
                        FontSize          = 100 - (5 * Model.MaximumAnswer),
                        Text              = text,
                        Padding           = new Thickness(0, 0, 0, 20),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var button = new Button
                    {
                        Background       = Model.AnswerButtonColor[answerNumber],
                        Command          = AnswerTest,
                        CommandParameter = Model.PossibleAnswers[answerNumber],
                        Content          = buttonText,
                        Height           = 100,
                        Margin           = new Thickness(5, 0, 5, 0),
                        Width            = (_mainWindow.Width - 20 - (10 * Model.MaximumAnswer)) / Model.MaximumAnswer
                    };

                    var noteText = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text                = $"{answerNumber + 1}",
                    };

                    stackPanel.Children.Add(button);
                    stackPanel.Children.Add(noteText);

                    _mainWindow.AnswerButtonArea.Children.Add(stackPanel);
                }
            }));

        /// <summary>
        /// Save all settings (data model) of this application
        /// </summary>
        internal void SaveSettings()
        {
            try
            {
                JsonHelper.WriteJson(_settingFileName, Model);
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
                Model = JsonHelper.ReadJson<MainModel>(_settingFileName);
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
