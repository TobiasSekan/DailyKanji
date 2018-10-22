﻿using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DailyKanji.Mvvm.ViewModel
{
    // TODO: Make kind of question chooseable (Hiragana, Katakana, ...)

    // TODO: Make colors chooseable
    // TODO: Make error highlight time changeable
    // TODO: Save and load setttings from JSON

    // TODO: Setable answer count (currently only five)

    // TODO: Ask wrong answerd question more times

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

            CreateNewTest();
            RemoveAnswerColors();

            _mainWindow = new MainWindow(this);
            _mainWindow.Show();
        }

        #endregion Public Constructors

        internal void CreateNewTest()
        {
            ChooseNewSign();
            ChooseNewPossibleAnswers();
            Model.IgnoreInput = false;
        }

        /// <summary>
        /// Choose a new sign for a new ask
        /// </summary>
        internal void ChooseNewSign()
        {
            if(Model.CurrentTest == null)
            {
                Model.CurrentTest = GetRandomTest();
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

            Debug.WriteLine(hiraganaOrKatakana);

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

            Model.CurrentTest.FailCount++;

            Model.WrongAnswers.Add(Model.CurrentTest);

            _mainWindow.Dispatcher.Invoke(new Action(() => SetAnswerColors()), DispatcherPriority.DataBind);

            var timer = new System.Timers.Timer(1500)
            {
                AutoReset = false
            };

            timer.Elapsed += (_, __) =>
            {
                _mainWindow.Dispatcher.Invoke(new Action(() => RemoveAnswerColors()), DispatcherPriority.DataBind);
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

            for(var answerNumber = 0; answerNumber < Model.MaximumAnswer; answerNumber++)
            {
                Model.AnswerButtonColor[answerNumber] = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Return a new random test
        /// </summary>
        /// <returns>A test</returns>
        internal TestModel GetRandomTest()
            => Model.TestList.ElementAtOrDefault(Model.Randomizer.Next(0, Model.TestList.Count)) ?? Model.TestList.FirstOrDefault();

        public ICommand AnswerNumber
            => new CommandHelper((parameter) => CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(Convert.ToInt32(parameter) - 1)?.Roomaji));
    }
}
