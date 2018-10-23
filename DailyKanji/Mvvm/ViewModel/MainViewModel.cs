﻿using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DailyKanji.Mvvm.ViewModel
{
    // TODO: Make kind of question chooseable (Hiragana, Katakana, ...)

    // TODO: Make colors chooseable
    // TODO: Make error highlight time changeable
    // TODO: Save and load setttings from JSON

    public sealed class MainViewModel
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

            _mainWindow = new MainWindow(this);

            CreateNewTest();
            RemoveAnswerColors();

            _mainWindow.Show();
        }

        #endregion Public Constructors

        #region Public Commands

        public ICommand AnswerNumber
            => new CommandHelper((parameter) => CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(Convert.ToInt32(parameter) - 1)?.Roomaji));

        public ICommand ChangeAnswerCount
            => new CommandHelper((_) => CreateNewTest());

        #endregion Public Commands

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
            Model.NewQuestionList.Clear();

            foreach(var question in Model.AllTestsList)
            {
                for(var repeatCount = 0; repeatCount < (question.WrongHiragana + question.WrongKatakana + 1); repeatCount++)
                {
                    Model.NewQuestionList.Add(question);
                }
            }

            Debug.WriteLine($"New question list count: {Model.NewQuestionList.Count}");
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
                Debug.WriteLine("New quest and last ask quest are the same -> choose a new quest");

                newQuest = GetRandomTest();
            }

            Model.CurrentTest = newQuest;

            var hiraganaOrKatakana = Model.Randomizer.Next(0, 2);

            Debug.WriteLine($"Choose quest: {(hiraganaOrKatakana == 0 ? "Hiragana" : "Katakana")}");

            Model.CurrentAskSign = hiraganaOrKatakana == 0 ? Model.CurrentTest.Hiragana : Model.CurrentTest.Katakana;
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
                var possbleAnswer = GetRandomTest();

                if(list.Contains(possbleAnswer))
                {
                    continue;
                }

                list.Add(possbleAnswer);
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
