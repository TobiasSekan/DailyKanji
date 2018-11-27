﻿using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DailyKanjiLogic.Mvvm.ViewModel
{
    public class MainBaseViewModel
    {
        #region Public Properties

        public MainBaseModel BaseModel { get; set; }

        #endregion Public Properties

        #region Public Constructor

        public MainBaseViewModel()
            => BaseModel = new MainBaseModel();

        #endregion Public Constructor

        #region Public Methods

        /// <summary>
        /// Reorder all tests by it own correct and wrong counters
        /// </summary>
        public void OrderAllTests()
            => BaseModel.AllTestsList
                = BaseModel.AllTestsList
                           .OrderByDescending(found => found.WrongHiraganaCount + found.WrongKatakanaCount)
                           .ThenByDescending(found => found.WrongHiraganaCount)
                           .ThenByDescending(found => found.WrongKatakanaCount)
                           .ThenByDescending(found => found.CorrectHiraganaCount + found.CorrectKatakanaCount)
                           .ThenByDescending(found => found.CorrectHiraganaCount)
                           .ThenByDescending(found => found.CorrectKatakanaCount).ToList();

        /// <summary>
        /// Build the test pool (wrong answered tests will add multiple)
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void BuildTestPool()
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
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana:
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
        /// <param name="newTest">The test for the new sign</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ChooseNewSign(TestBaseModel newTest)
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
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana:
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
        /// Return a new random test
        /// </summary>
        /// <param name="onlyOneRoomajiCharacter">(Optional) Indicate that only a test that have a roomaji character with length one will return</param>
        /// <returns>A test</returns>
        public TestBaseModel GetRandomTest(in bool onlyOneRoomajiCharacter = false)
            => onlyOneRoomajiCharacter
                ? BaseModel.TestPool.Where(found => found.Roomaji.Length == 1)
                                    .ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count))
                : BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        public void ChooseNewPossibleAnswers()
        {
            var firstTestChrachter  = BaseModel.CurrentTest.Roomaji.FirstOrDefault();
            var secondTestChrachter = BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(1);
            var ThirdTestChrachter  = BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(2);

            var tryAddCount = 0;
            var list        = new ObservableCollection<TestBaseModel>
            {
                BaseModel.CurrentTest
            };

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
                && !possibleAnswer.Roomaji.Contains(firstTestChrachter)
                && !possibleAnswer.Roomaji.Contains(secondTestChrachter)
                && !possibleAnswer.Roomaji.Contains(ThirdTestChrachter))
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
        /// Return a text for a answer, based on the selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answerNumber">The number of the answer</param>
        /// <returns>A text for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetAnswerText(in byte answerNumber)
        {
            switch(BaseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                case TestType.HiraganaToRoomaji:
                case TestType.KatakanaToRoomaji:
                    return BaseModel.PossibleAnswers[answerNumber].Roomaji;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.RoomajiToHiragana:
                case TestType.KatakanaToHiragana:
                    return BaseModel.PossibleAnswers[answerNumber].Hiragana;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign != BaseModel.CurrentTest.Katakana:
                case TestType.RoomajiToKatakana:
                case TestType.HiraganaToKatakana:
                    return BaseModel.PossibleAnswers[answerNumber].Katakana;

                case TestType.RoomajiToHiraganaOrKatakana:
                    return BaseModel.Randomizer.Next(0, 2) == 0
                            ? BaseModel.PossibleAnswers[answerNumber].Hiragana
                            : BaseModel.PossibleAnswers[answerNumber].Katakana;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        /// <summary>
        /// Return a hint for a answer,
        /// based on the selected <see cref="HintType"/> and selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answerNumber">The number of the answer</param>
        /// <returns>A hint for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetAnswerHint(in byte answerNumber)
        {
            switch(BaseModel.SelectedHintType)
            {
                case HintType.OnlyRoomaji:
                    return BaseModel.PossibleAnswers[answerNumber].Roomaji;

                case HintType.OnlyHiragana:
                    return BaseModel.PossibleAnswers[answerNumber].Hiragana;

                case HintType.OnlyKatakana:
                    return BaseModel.PossibleAnswers[answerNumber].Katakana;

                case HintType.BasedOnAskSign:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedHintType), "Hint type not supported");
            }

            switch(BaseModel.SelectedTestType)
            {
                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                    return BaseModel.PossibleAnswers[answerNumber].Roomaji;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    return BaseModel.PossibleAnswers[answerNumber].Hiragana;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign != BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign != BaseModel.CurrentTest.Hiragana:
                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    return BaseModel.PossibleAnswers[answerNumber].Katakana;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        /// <summary>
        /// Count the result of a test, based on the given answer and the selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer of a test</param>
        /// <param name="answerList">A list with all answers</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CountAnswerResult(in TestBaseModel answer)
        {
            var answerTime = DateTime.UtcNow - BaseModel.TestStartTime;

            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
            {
                switch(BaseModel.SelectedTestType)
                {
                    case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                    case TestType.HiraganaToRoomaji:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakana:
                        BaseModel.CurrentTest.CompleteAnswerTimeForCorrectHiragana += answerTime;
                        BaseModel.CurrentTest.CorrectHiraganaCount++;
                        break;

                    case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        BaseModel.CurrentTest.CompleteAnswerTimeForCorrectKatakana += answerTime;
                        BaseModel.CurrentTest.CorrectKatakanaCount++;
                        break;

                    // TODO: fix correct counting for wrong answers on test type "RoomajiToHiraganaOrKatakana"
                    //case TestType.RoomajiToHiraganaOrKatakana when IsKatakana(answer):
                    //case TestType.RoomajiToHiraganaOrKatakana when IsHiragana(answer):
                    case TestType.RoomajiToHiraganaOrKatakana:
                        BaseModel.CurrentTest.CompleteAnswerTimeForCorrectHiragana += answerTime;
                        BaseModel.CurrentTest.CorrectHiraganaCount++;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                }

                return;
            }

            switch(BaseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.RoomajiToHiragana:
                case TestType.HiraganaToKatakana:
                    BaseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += answerTime;
                    BaseModel.CurrentTest.WrongHiraganaCount++;
                    break;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.KatakanaToRoomaji:
                case TestType.RoomajiToKatakana:
                case TestType.KatakanaToHiragana:
                    BaseModel.CurrentTest.CompleteAnswerTimeForWrongKatakana += answerTime;
                    BaseModel.CurrentTest.WrongKatakanaCount++;
                    break;

                    // TODO: fix correct counting for wrong answers on test type "RoomajiToHiraganaOrKatakana"
                    //case TestType.RoomajiToHiraganaOrKatakana when IsKatakana(answer):
                    //case TestType.RoomajiToHiraganaOrKatakana when IsHiragana(answer):
                    case TestType.RoomajiToHiraganaOrKatakana:
                    BaseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += answerTime;
                    BaseModel.CurrentTest.WrongHiraganaCount++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        /// <summary>
        /// Rest the statistic
        /// </summary>
        /// <param name="resetType">The type of the reset (All, OnlyAllCorrect, OnlyAllWrong, etc.)</param>
        public void ResetCompleteStatistic(in ResetType resetType)
        {
            foreach(var test in BaseModel.AllTestsList)
            {
                switch(resetType)
                {
                    case ResetType.All:
                        test.CorrectHiraganaCount                 = 0;
                        test.CorrectKatakanaCount                 = 0;
                        test.WrongHiraganaCount                   = 0;
                        test.WrongKatakanaCount                   = 0;
                        test.CompleteAnswerTimeForCorrectHiragana = new TimeSpan();
                        test.CompleteAnswerTimeForWrongHiragana   = new TimeSpan();
                        test.CompleteAnswerTimeForCorrectKatakana = new TimeSpan();
                        test.CompleteAnswerTimeForWrongKatakana   = new TimeSpan();
                        break;

                    case ResetType.OnlyCorrectAll:
                        test.CorrectHiraganaCount                 = 0;
                        test.CorrectKatakanaCount                 = 0;
                        test.CompleteAnswerTimeForCorrectHiragana = new TimeSpan();
                        test.CompleteAnswerTimeForCorrectKatakana = new TimeSpan();
                        break;

                    case ResetType.OnlyCorrectHiragana:
                        test.CorrectHiraganaCount                 = 0;
                        test.CompleteAnswerTimeForCorrectHiragana = new TimeSpan();
                        break;

                    case ResetType.OnlyCorrectKatakana:
                        test.CorrectKatakanaCount                 = 0;
                        test.CompleteAnswerTimeForCorrectKatakana = new TimeSpan();
                        break;

                    case ResetType.OnlyWrongAll:
                        test.WrongHiraganaCount                 = 0;
                        test.WrongKatakanaCount                 = 0;
                        test.CompleteAnswerTimeForWrongHiragana = new TimeSpan();
                        test.CompleteAnswerTimeForWrongKatakana = new TimeSpan();
                        break;

                    case ResetType.OnlyWrongHiragana:
                        test.WrongHiraganaCount                 = 0;
                        test.CompleteAnswerTimeForWrongHiragana = new TimeSpan();
                        break;

                    case ResetType.OnlyWrongKatakana:
                        test.WrongKatakanaCount                 = 0;
                        test.CompleteAnswerTimeForWrongKatakana = new TimeSpan();
                        break;
                }
            }
        }

        /// <summary>
        /// Test if the given sign is a Roomaji sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><c>true</c> if the given sign is a Roomaji, otherwise <c>false</c></returns>
        public bool IsRoomaji(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Roomaji == signToTest);

        /// <summary>
        /// Test if the given sign is a Hiragana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><c>true</c> if the given sign is a Hiragana, otherwise <c>false</c></returns>
        public bool IsHiragana(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Hiragana == signToTest);

        /// <summary>
        /// Test if the given sign is a Katakana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><c>true</c> if the given sign is a Katakana, otherwise <c>false</c></returns>
        public bool IsKatakana(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Katakana == signToTest);
    }

    #endregion Public Methods
}
