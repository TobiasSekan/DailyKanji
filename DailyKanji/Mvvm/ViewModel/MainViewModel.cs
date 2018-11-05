using DailyKanji.Enumerations;
using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DailyKanji.Mvvm.ViewModel
{
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers

    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this 
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers

    // TODO: Change test order so that all tests will be ask (based on ask counter)

    // TODO: Add new answers sub-menu (show current answer inside menu entry with shortcut)

    // TODO: Add tests for Roomaji to Katakana and Roomaji to Hiragana

    // TODO: Recalculate buttons (button width), when window is resized

    // TODO: Make colours choose-able

    // TODO: visible timer in 0.1 second (can be deactivated via menu)
    // TODO: show average answer time for the current sign (calculate with the current running answer time)

    // TODO: Export (XLSX, CSV, JSON, XML)
    // TODO: Import ???

    public sealed partial class MainViewModel
    {
        #region Public Properties

        public MainModel Model { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private readonly MainWindow _mainWindow;

        #endregion Private Fields

        #region Public Constructors

        internal MainViewModel()
        {
            Model = new MainModel();

            LoadSettings();

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != Model.AllTestsList?.Count())
            {
                Model.AllTestsList = list;
            }

            Model.Randomizer        = new Random();
            Model.AnswerButtonColor = new ObservableCollection<Brush>();
            Model.PossibleAnswers   = new ObservableCollection<TestBaseModel>();
            Model.NewQuestionList   = new Collection<TestModel>();

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
                                      .ThenByDescending(found => found.CorrectKatakanaCount);

            BuildNewQuestionList();
            ChooseNewSign();
            ChooseNewPossibleAnswers();
            BuildAnswerButtons();

            Model.IgnoreInput   = false;
            Model.TestStartTime = DateTime.UtcNow;
        }

        internal void BuildNewQuestionList()
        {
            var questionList = new Collection<TestModel>();

            foreach(var question in Model.AllTestsList)
            {
                if(Model.MainTestType != TestType.KatakanaToRoomaji)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongHiraganaCount + 1; repeatCount++)
                    {
                        questionList.Add(new TestModel(question, TestType.HiraganaToRoomaji));
                    }
                }

                if(Model.MainTestType != TestType.HiraganaToRoomaji)
                {
                    for(var repeatCount = 0; repeatCount < question.WrongKatakanaCount + 1; repeatCount++)
                    {
                        questionList.Add(new TestModel(question, TestType.KatakanaToRoomaji));
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
            if(Model.CurrentTest == null)
            {
                Model.CurrentTest    = GetRandomTest();
                Model.CurrentAskSign = Model.CurrentTest.TestType == TestType.HiraganaToRoomaji
                                            ? Model.CurrentTest.Hiragana
                                            : Model.CurrentTest.Katakana;
                return;
            }

            var newQuest = GetRandomTest();

            while(newQuest.Roomaji == Model.CurrentTest.Roomaji)
            {
                newQuest = GetRandomTest();
            }

            Model.CurrentTest    = newQuest;
            Model.CurrentAskSign = Model.CurrentTest.TestType == TestType.HiraganaToRoomaji
                                        ? Model.CurrentTest.Hiragana
                                        : Model.CurrentTest.Katakana;
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

            while(list.Count < Model.MaximumAnswer)
            {
                var possibleAnswer = GetRandomTest();

                if(list.Any(found => found.Roomaji == possibleAnswer.Roomaji))
                {
                    continue;
                }

                if(!Model.SimilarAnswers || Model.CurrentTest.Roomaji.Length == 1)
                {
                    list.Add(possibleAnswer);
                    continue;
                }

                if(!possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.FirstOrDefault())
                && !possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.ElementAtOrDefault(1))
                && !possibleAnswer.Roomaji.Contains(Model.CurrentTest.Roomaji.ElementAtOrDefault(2)))
                {
                    continue;
                }

                list.Add(possibleAnswer);
            }

            list.Shuffle();

            Model.PossibleAnswers = list;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer"></param>
        internal void CheckAnswer(string answer)
        {
            if(Model.IgnoreInput)
            {
                return;
            }

            Model.IgnoreInput = true;

            var answerTime = DateTime.UtcNow - Model.TestStartTime;
            var test       = Model.AllTestsList.FirstOrDefault(found => found.Roomaji == Model.CurrentTest.Roomaji);

            if(test == null)
            {
                throw new ArgumentNullException("test", "Test not found");
            }

            if(answer == Model.CurrentTest.Roomaji)
            {
                if(Model.CurrentAskSign == test.Hiragana)
                {
                    test.CompleteAnswerTimeForHiragana += answerTime;
                    test.CorrectHiraganaCount++;
                }

                if(Model.CurrentAskSign == test.Katakana)
                {
                    test.CompleteAnswerTimeForKatakana += answerTime;
                    test.CorrectKatakanaCount++;
                }

                CreateNewTest();
                return;
            }

            if(Model.CurrentAskSign == test.Hiragana)
            {
                test.CompleteAnswerTimeForHiragana += answerTime;
                test.WrongHiraganaCount++;
            }

            if(Model.CurrentAskSign == test.Katakana)
            {
                test.CompleteAnswerTimeForKatakana += answerTime;
                test.WrongKatakanaCount++;
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
        /// <returns>A test</returns>
        internal TestModel GetRandomTest()
            => Model.NewQuestionList.ElementAtOrDefault(Model.Randomizer.Next(0, Model.NewQuestionList.Count)) ?? Model.NewQuestionList.FirstOrDefault();

        /// <summary>
        /// Build all answer buttons (with text and colours)
        /// </summary>
        internal void BuildAnswerButtons()
            => _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerButtonArea.Children.Clear();

                for(var answerNumber = 0; answerNumber < Model.MaximumAnswer; answerNumber++)
                {
                    var stackPanel = new StackPanel();

                    var buttonText = new TextBlock
                    {
                        FontSize          = 100 - (5 * Model.MaximumAnswer),
                        Text              = Model.PossibleAnswers[answerNumber].Roomaji,
                        Padding           = new Thickness(0, 0, 0, 20),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var button = new Button
                    {
                        Background       = Model.AnswerButtonColor[answerNumber],
                        Command          = AnswerNumber,
                        CommandParameter = $"{answerNumber + 1}",
                        Content          = buttonText,
                        Height           = 100,
                        Margin           = new Thickness(5, 0, 5, 0),
                        Width            = (980 - (10 * Model.MaximumAnswer)) / Model.MaximumAnswer
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

        internal void SaveSettings()
        {
            try
            {
                JsonHelper.WriteJson("settings.json", Model);
            }
            catch(Exception exception)
            {
                MessageBox.Show("Can't save settings" + Environment.NewLine + Environment.NewLine + exception.ToString(),
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        internal void LoadSettings()
        {
            try
            {
                Model = JsonHelper.ReadJson<MainModel>("settings.json");
            }
            catch(Exception exception)
            {
                MessageBox.Show("Can't load settings" + Environment.NewLine + Environment.NewLine + exception.ToString(),
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        #endregion Internal Methods
    }
}
