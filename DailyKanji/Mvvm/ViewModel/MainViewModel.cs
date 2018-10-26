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
    // TODO: Menu-bar (missing events)

    // TODO: Recalculate buttons (button width), when window is resized

    // TODO: Make kind of question choose-able (Hiragana, Katakana, ...) + menu + status-bar

    // TODO: Make colours choose-able
    // TODO: Make error highlight time changeable

    // TODO: Save and load settings from JSON

    public sealed partial class MainViewModel
    {
        #region Public Properties

        public MainModel Model { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly MainWindow _mainWindow;

        #endregion Private Fields

        #region Public Constructors

        internal MainViewModel()
        {
            Model = new MainModel();

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                Model.AnswerButtonColor.Add(new SolidColorBrush(Colors.Transparent));
            }

            Model.SimilarAnswers = true;

            BuildNewQuestionList();
            ChooseNewSign();

            _mainWindow = new MainWindow(this);

            CreateNewTest();
            RemoveAnswerColors();

            _mainWindow.Show();
        }

        #endregion Public Constructors

        #region Internal Methods

        /// <summary>
        /// Create a new test with new question and new possible answers
        /// </summary>
        internal void CreateNewTest()
        {
            BuildNewQuestionList();
            ChooseNewSign();
            ChooseNewPossibleAnswers();
            BuildAnswerButtons();
            Model.IgnoreInput = false;
        }

        internal void BuildNewQuestionList()
        {
            var questionList = new Collection<TestModel>();

            foreach(var question in Model.AllTestsList)
            {
                for(var repeatCount = 0; repeatCount < (question.WrongHiragana + question.WrongKatakana + 1); repeatCount++)
                {
                    questionList.Add(question);
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
                Model.CurrentAskSign = Model.Randomizer.Next(0, 1) == 0 ? Model.CurrentTest.Hiragana : Model.CurrentTest.Katakana;
                return;
            }

            var newQuest = GetRandomTest();

            while(newQuest.Roomaji == Model.CurrentTest.Roomaji)
            {
                newQuest = GetRandomTest();
            }

            Model.CurrentTest    = newQuest;
            Model.CurrentAskSign = Model.Randomizer.Next(0, 2) == 0 ? Model.CurrentTest.Hiragana : Model.CurrentTest.Katakana;
        }

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        internal void ChooseNewPossibleAnswers()
        {
            var list = new ObservableCollection<TestModel>
            {
                Model.CurrentTest
            };

            while(list.Count < Model.MaximumAnswer)
            {
                var possibleAnswer = GetRandomTest();

                if(list.Contains(possibleAnswer))
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

            if(answer == Model.CurrentTest.Roomaji)
            {
                Model.RightAnswerCount++;
                CreateNewTest();
                return;
            }

            if(Model.CurrentAskSign == Model.CurrentTest.Hiragana)
            {
                Model.CurrentTest.WrongHiragana++;
            }

            if(Model.CurrentAskSign == Model.CurrentTest.Katakana)
            {
                Model.CurrentTest.WrongKatakana++;
            }

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                SetAnswerColors();
                BuildAnswerButtons();
            }));

            var timer = new System.Timers.Timer(1500)
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
                        Text     = Model.PossibleAnswers[answerNumber].Roomaji,
                        FontSize = 50
                    };

                    var button = new Button
                    {
                        Content          = buttonText,
                        Height           = 100,
                        Width            = (980 - (10 * Model.MaximumAnswer)) / Model.MaximumAnswer,
                        Margin           = new Thickness(5, 0, 5, 0),
                        Background       = Model.AnswerButtonColor[answerNumber],
                        CommandParameter = $"{answerNumber + 1}",
                        Command          = AnswerNumber
                    };

                    var noteText = new TextBlock
                    {
                        Text = $"{answerNumber + 1}",
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    stackPanel.Children.Add(button);
                    stackPanel.Children.Add(noteText);

                    _mainWindow.AnswerButtonArea.Children.Add(stackPanel);
                }
            }));

        #endregion Internal Methods
    }
}
