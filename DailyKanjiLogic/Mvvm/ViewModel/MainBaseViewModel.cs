using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("DailyKanjiLogicTest")]

namespace DailyKanjiLogic.Mvvm.ViewModel
{
    public class MainBaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Data model that contain all static and changeable runtime data of the program logic
        /// </summary>
        private MainBaseModel BaseModel { get; }

        #endregion Public Properties

        #region Public Constructor

        public MainBaseViewModel(in MainBaseModel baseModel)
        {
            BaseModel = baseModel;

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != BaseModel.AllTestsList?.Count())
            {
                BaseModel.AllTestsList = list.ToList();
            }

            BaseModel.Randomizer        = new Random();
            BaseModel.PossibleAnswers   = new Collection<TestBaseModel>();
            BaseModel.TestPool          = new Collection<TestBaseModel>();
            BaseModel.AnswerButtonColor = new ObservableCollection<Color>();
            BaseModel.HintTextColor     = new ObservableCollection<Color>();
            BaseModel.HighlightTimer    = new ManualResetEvent(false);
            BaseModel.ProgressBarColor  = ColorHelper.ProgressBarColor;

            var answerButtonColorList = new List<Color>(10);
            var hintTextColorList     = new List<Color>(10);

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                answerButtonColorList.Add(ColorHelper.TransparentColor);
                hintTextColorList.Add(ColorHelper.TransparentColor);
            }

            BaseModel.AnswerButtonColor = answerButtonColorList;
            BaseModel.HintTextColor     = hintTextColorList;

            BuildTestPool();

            ChooseNewSign(GetRandomKanaTest());
        }

        #endregion Public Constructor

        #region Public Methods

        /// <summary>
        /// Reorder all tests by it own correct and wrong counters
        /// </summary>
        public void OrderAllTests()
            => BaseModel.AllTestsList = BaseModel.AllTestsList
                .OrderByDescending(found => found.WrongnessCounter)
                .ThenByDescending(found => found.WrongHiraganaCount + found.WrongKatakanaCount)
                .ThenByDescending(found => found.WrongHiraganaCount)
                .ThenByDescending(found => found.WrongKatakanaCount)
                .ThenByDescending(found => found.CorrectHiraganaCount + found.CorrectKatakanaCount)
                .ThenByDescending(found => found.CorrectHiraganaCount)
                .ThenByDescending(found => found.CorrectKatakanaCount)
                .ToList();

        /// <summary>
        /// Build the test pool (wrong answered tests will add multiple)
        /// </summary>
        public void BuildTestPool()
        {
            var testPool = new Collection<TestBaseModel>();

            foreach(var test in BaseModel.AllTestsList)
            {
                if(!BaseModel.SelectedKanaType.HasFlag(test.Type))
                {
                    continue;
                }

                var repeatCounter = test.WrongnessCounter * 10;

                for(var count = 0; count <= repeatCounter; count++)
                {
                    testPool.Add(test);
                }
            }

            BaseModel.TestPool = testPool;
        }

        /// <summary>
        /// Choose a new sign for a new ask
        /// </summary>
        /// <param name="newTest">The test for the new sign</param>
        /// <exception cref="ArgumentOutOfRangeException">Test type not supported or </exception>
        public void ChooseNewSign(in TestBaseModel newTest)
        {
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

                case TestType.AllToAll:
                    var randomNumber = BaseModel.Randomizer.Next(0, 3);

                    BaseModel.CurrentAskSign = randomNumber switch
                    {
                        0 => BaseModel.CurrentTest.Hiragana,
                        1 => BaseModel.CurrentTest.Katakana,
                        2 => BaseModel.CurrentTest.Roomaji,
                        _ => throw new ArgumentOutOfRangeException(
                            nameof(randomNumber),
                            randomNumber,
                            "is not between 0 and 2"),
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(BaseModel.SelectedTestType),
                        BaseModel.SelectedTestType,
                        "Test type not supported");
            }
        }

        /// <summary>
        /// Return a random kana test and avoid that the test is the same as the current selected test
        /// </summary>
        /// <returns>A kana test</returns>
        public TestBaseModel GetRandomKanaTest()
        {
            if(!BaseModel.TestPool.Any())
            {
                return TestBaseModel.EmptyTest;
            }

            var testPollCount = BaseModel.TestPool.Count();
            var newTest       = BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, testPollCount));

            if(BaseModel.CurrentTest is null)
            {
                return newTest;
            }

            while(newTest.Equals(BaseModel.CurrentTest))
            {
                newTest = BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, testPollCount));
            }

            return newTest;
        }

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        public void ChooseNewPossibleAnswers()
        {
            var possibleAnswers = new ObservableCollection<TestBaseModel>
            {
                // add correct answer for this test to list with possible answers
                BaseModel.CurrentTest,
            };

            var similarAnswerList = BaseModel.SimilarAnswers
                ? KanaHelper.GetSimilarKana(BaseModel.TestPool.Distinct(), BaseModel.CurrentTest, BaseModel.CurrentTest.AnswerType)
                : BaseModel.TestPool.Distinct();

            var allAnswerList = BaseModel.ShowOnlySameKanaOnAnswers
                ? KanaHelper.GetSameKana(similarAnswerList, BaseModel.CurrentTest).ToList()
                : similarAnswerList.ToList();

            allAnswerList.Remove(BaseModel.CurrentTest);
            allAnswerList.Shuffle();

            while(possibleAnswers.Count < BaseModel.MaximumAnswers)
            {
                if(allAnswerList.Count > 0)
                {
                    var possibleAnswer = allAnswerList.ElementAtOrDefault(BaseModel.Randomizer.Next(0, allAnswerList.Count));
                    possibleAnswers.Add(possibleAnswer);
                    allAnswerList.Remove(possibleAnswer);
                    continue;
                }

                possibleAnswers.Add(TestBaseModel.EmptyTest);
            }

            possibleAnswers.Shuffle();

            BaseModel.PossibleAnswers = possibleAnswers;
        }

        /// <summary>
        /// Return a text for a answer, based on the given <see cref="AnswerType"/>
        /// </summary>
        /// <param name="answer">The answer, that should have a text</param>
        /// <param name="answerType">The type of the answer</param>
        /// <returns>A text for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException">Answer type not supported</exception>
        public static string GetAnswerText(in TestBaseModel answer, in AnswerType answerType)
        {
            answer.AnswerType = answerType;

            return answerType switch
            {
                AnswerType.Roomaji  => answer.Roomaji,
                AnswerType.Hiragana => answer.Hiragana,
                AnswerType.Katakana => answer.Katakana,
                _                   => throw new ArgumentOutOfRangeException(
                                        nameof(answerType),
                                        answerType,
                                        "Answer type not supported"),
            };
        }

        /// <summary>
        /// Return a hint for a answer, based on the selected <see cref="HintType"/> and selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer that should have a hint</param>
        /// <returns>A hint for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException">Hint type not supported</exception>
        public string GetAnswerHint(in TestBaseModel answer)
            => BaseModel.SelectedHintType switch
            {
                HintType.AlwaysInRoomaji => answer.Roomaji,
                HintType.AlwaysInHiragana => answer.Hiragana,
                HintType.AlwaysInKatakana => answer.Katakana,
                HintType.BasedOnAskSign => GetAnswerHintBasedOnAskSign(answer),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(BaseModel.SelectedHintType),
                    BaseModel.SelectedHintType,
                    "Hint type not supported"),
            };

        /// <summary>
        /// Count the result of a test, based on the given answer and the selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer of a test</param>
        /// <exception cref="ArgumentOutOfRangeException">Test type combination not supported</exception>
        public void CountAnswerResult(in TestBaseModel answer)
        {
            switch(BaseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Hiragana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.RoomajiToHiragana:
                case TestType.HiraganaToKatakana:
                    CountWrongOrCorrectHiragana(answer);
                    return;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Katakana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Katakana:
                case TestType.KatakanaToRoomaji:
                case TestType.RoomajiToKatakana:
                case TestType.KatakanaToHiragana:
                    CountWrongOrCorrectKatakana(answer);
                    return;

                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Unknown:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Unknown:
                    CountWrongHiarganaOrKatakana();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(BaseModel.SelectedTestType),
                        BaseModel,
                        "Test type combination not supported\n"
                        + $"SelectedTestType:     [{BaseModel.SelectedTestType}]\n"
                        + $"CurrentAskSign:       [{BaseModel.CurrentAskSign}]\n"
                        + $"CurrentTest.Katakana: [{BaseModel.CurrentTest.Katakana}]\n"
                        + $"CurrentTest.Hiragana: [{BaseModel.CurrentTest.Hiragana}]\n"
                        + $"answer.AnswerType:    [{answer.AnswerType}]");
            }

            // Reset the AnswerType - only for testing / debugging
            answer.AnswerType = AnswerType.Unknown;
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
        /// <returns><see langword="true"/> if the given sign is a Roomaji, otherwise <see langword="false"/></returns>
        public bool IsRoomaji(in string signToTest)
        {
            var romajiToTest = signToTest;

            return BaseModel.AllTestsList.Any(found => found.Roomaji == romajiToTest);
        }

        /// <summary>
        /// Test if the given sign is a Hiragana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Hiragana, otherwise <see langword="false"/></returns>
        public bool IsHiragana(in string signToTest)
        {
            var hiarganaToTest = signToTest;

            return BaseModel.AllTestsList.Any(found => found.Hiragana == hiarganaToTest);
        }

        /// <summary>
        /// Test if the given sign is a Katakana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Katakana, otherwise <see langword="false"/></returns>
        public bool IsKatakana(in string signToTest)
        {
            var katakanaToTest = signToTest;

            return BaseModel.AllTestsList.Any(found => found.Katakana == katakanaToTest);
        }

        /// <summary>
        /// Try to load all settings
        /// </summary>
        /// <param name="path">The path to the setting file</param>
        /// <param name="baseModel">The <see cref="MainBaseModel"/> with all loaded settings</param>
        /// <param name="exception">The possible thrown exception until the setting file is load</param>
        /// <returns><see langword="true"/> if the setting file could be load, otherwise <see langword="false"/></returns>
        public static bool TryLoadSettings(in string path, out MainBaseModel baseModel, out Exception? exception)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                baseModel = new MainBaseModel();
                exception = new ArgumentException("Settings path is empty", nameof(path));
                return false;
            }

            if(!File.Exists(path))
            {
                baseModel = new MainBaseModel();
                exception = new FileNotFoundException("Settings file not found", path);
                return false;
            }

            try
            {
                baseModel = JsonHelper.ReadJson<MainBaseModel>(path);

                baseModel.CheckAndFixValues();

                exception = null;
                return true;
            }
            catch(Exception thrownException)
            {
                baseModel = new MainBaseModel();
                exception = thrownException;
                return false;
            }
        }

        /// <summary>
        /// Try to save all settings
        /// </summary>
        /// <param name="path">The path to the setting file</param>
        /// <param name="exception">The possible thrown exception until the setting file is load</param>
        /// <returns><see langword="true"/> if the setting file could be save, otherwise <see langword="false"/></returns>
        public bool TrySaveSettings(in string path, out Exception? exception)
        {
            try
            {
                JsonHelper.WriteJson(path, BaseModel);
                exception = null;
                return true;
            }
            catch(Exception jsonException)
            {
                exception = jsonException;
                return false;
            }
        }

        /// <summary>
        /// Set the normal colors for all elements
        /// </summary>
        public void SetNormalColors()
        {
            BaseModel.CurrentAskSignColor = ColorHelper.TransparentColor;
            BaseModel.ProgressBarColor    = ColorHelper.ProgressBarColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                BaseModel.AnswerButtonColor[answerNumber] = ColorHelper.TransparentColor;
                BaseModel.HintTextColor[answerNumber]     = ColorHelper.TransparentColor;
            }

            BaseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Set the highlight colors for all elements
        /// </summary>
        /// <param name="answer">The answers of the current test</param>
        public void SetHighlightColors(in TestBaseModel answer)
        {
            BaseModel.CurrentAskSignColor = answer.Equals(BaseModel.CurrentTest) ? ColorHelper.CorrectColor : ColorHelper.ErrorColor;
            BaseModel.ProgressBarColor    = answer.Equals(BaseModel.CurrentTest) ? ColorHelper.CorrectColor : ColorHelper.ErrorColor;

            for(var answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
            {
                SetHintTextColors(answerNumber, answer);

                if(BaseModel.AnswerButtonColor.ElementAtOrDefault(answerNumber) == default)
                {
                    continue;
                }

                var possibleAnswer = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
                if(possibleAnswer.Equals(BaseModel.CurrentTest))
                {
                    BaseModel.AnswerButtonColor[answerNumber] = ColorHelper.CorrectColor;
                    continue;
                }

                if(possibleAnswer.Equals(answer))
                {
                    BaseModel.AnswerButtonColor[answerNumber] = ColorHelper.ErrorColor;
                    continue;
                }

                if(BaseModel.AnswerButtonColor[answerNumber] == ColorHelper.MarkedColor)
                {
                    continue;
                }

                BaseModel.AnswerButtonColor[answerNumber] = ColorHelper.TransparentColor;
            }

            BaseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Set or remove the highlight color for one answer (when highlight color is set the color will be removed)
        /// </summary>
        /// <param name="answer">The answer to highlight</param>
        public void SetOrRemoveHighlightColorToOneAnswer(in TestBaseModel answer)
        {
            var answerNumber = GetAnswerNumber(answer);

            BaseModel.AnswerButtonColor[answerNumber] = BaseModel.AnswerButtonColor[answerNumber] != ColorHelper.MarkedColor
                ? ColorHelper.MarkedColor
                : ColorHelper.TransparentColor;

            BaseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Check the given answer and count the result
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <returns><see langword="true"/> if the answer was correct, otherwise <see langword="false"/></returns>
        public bool CheckAndCountAnswer(in TestBaseModel answer)
        {
            BaseModel.IgnoreInput  = true;
            BaseModel.PreviousTest = BaseModel.CurrentTest;

            CountAnswerResult(answer);

            return answer.Equals(BaseModel.CurrentTest);
        }

        /// <summary>
        /// Count one correct or wrong result for a Hiragana question, based on the given answer
        /// </summary>
        /// <param name="answer">The answer for the counting</param>
        public void CountWrongOrCorrectHiragana(in TestBaseModel answer)
        {
            if(answer.Equals(BaseModel.CurrentTest))
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForCorrectHiragana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.CorrectHiraganaCount++;
            }
            else
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.WrongHiraganaCount++;
            }
        }

        /// <summary>
        /// Count one correct or wrong result for a Katakana question, based on the given answer
        /// </summary>
        /// <param name="answer">The answer for the counting</param>
        public void CountWrongOrCorrectKatakana(in TestBaseModel answer)
        {
            if(answer.Equals(BaseModel.CurrentTest))
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForCorrectKatakana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.CorrectKatakanaCount++;
            }
            else
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForWrongKatakana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.WrongKatakanaCount++;
            }
        }

        /// <summary>
        /// Count one wrong result for a unknown or skipped answer
        /// </summary>
        public void CountWrongHiarganaOrKatakana()
        {
            // When no answer is selected and the ask sign is in Roomaji,
            // we don't know which error counter we should increase. So we decide the coincidence
            if(BaseModel.Randomizer.Next(0, 2) == 0)
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.WrongHiraganaCount++;
            }
            else
            {
                BaseModel.CurrentTest.CompleteAnswerTimeForWrongKatakana += BaseModel.AnswerTime;
                BaseModel.CurrentTest.WrongKatakanaCount++;
            }
        }

        /// <summary>
        /// Return the answer type for the current selected test, based on the current <see cref="BaseModel.SelectedTestType"/>
        /// </summary>
        /// <returns>The answer type for the current test</returns>
        /// <exception cref="ArgumentOutOfRangeException">Test type not supported</exception>
        public AnswerType GetAnswerType()
        {
            switch(BaseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                case TestType.HiraganaToRoomaji:
                case TestType.KatakanaToRoomaji:
                    return AnswerType.Roomaji;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.RoomajiToHiragana:
                case TestType.KatakanaToHiragana:
                    return AnswerType.Hiragana;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign != BaseModel.CurrentTest.Katakana:
                case TestType.RoomajiToKatakana:
                case TestType.HiraganaToKatakana:
                    return AnswerType.Katakana;

                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji:
                    return (BaseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Hiragana : AnswerType.Katakana;

                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                    return (BaseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Katakana : AnswerType.Roomaji;

                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                    return (BaseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Hiragana : AnswerType.Roomaji;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(BaseModel.SelectedTestType),
                        BaseModel.SelectedTestType,
                        "Test type not supported");
            }
        }

        /// <summary>
        /// Do all things to prepare a new test and all possible answers (no surface changes)
        /// </summary>
        public void PrepareNewTest()
        {
            OrderAllTests();
            BuildTestPool();
            ChooseNewSign(GetRandomKanaTest());
            ChooseNewPossibleAnswers();
        }

        /// <summary>
        /// Refresh and set highlight for the statistic values on the main window
        /// </summary>
        /// <param name="test">The test with the statistics values before the answer was count</param>
        public void RefreshAndSetHighlightForStatisticValues(in TestBaseModel test)
        {
            var correctBefore = test.CorrectHiraganaCount + test.CorrectKatakanaCount;
            var correctAfter  = BaseModel.CurrentTest.CorrectHiraganaCount + BaseModel.CurrentTest.CorrectKatakanaCount;

            var wrongBefore = test.WrongHiraganaCount + test.WrongKatakanaCount;
            var wrongAfter  = BaseModel.CurrentTest.WrongHiraganaCount + BaseModel.CurrentTest.WrongKatakanaCount;

            var answerTimeBefore = test.AverageAnswerTimeForHiragana + test.AverageAnswerTimeForKatakana;
            var answerTimeAfter  = BaseModel.CurrentTest.AverageAnswerTimeForHiragana + BaseModel.CurrentTest.AverageAnswerTimeForKatakana;

            if(correctBefore != correctAfter)
            {
                BaseModel.HighlightCorrectCounter = true;
                BaseModel.CorrectCountIndicator   = correctBefore < correctAfter ? "⇧" : "⇩";
            }

            if(wrongBefore != wrongAfter)
            {
                BaseModel.HighlightWrongCounter = true;
                BaseModel.WrongCountIndicator   = wrongBefore < wrongAfter ? "⇧" : "⇩";
            }

            if(answerTimeBefore != answerTimeAfter)
            {
                BaseModel.HighlightAnswerTime        = true;
                BaseModel.AverageAnswerTimeIndicator = answerTimeBefore < answerTimeAfter ? "⇧" : "⇩";
            }

            BaseModel.OnPropertyChangedOnlyForStatistics();
        }

        /// <summary>
        /// Reset all highlight properties
        /// </summary>
        public void ResetHighlight()
        {
            BaseModel.HighlightCorrectCounter = false;
            BaseModel.HighlightWrongCounter   = false;
            BaseModel.HighlightAnswerTime     = false;

            BaseModel.CorrectCountIndicator      = string.Empty;
            BaseModel.WrongCountIndicator        = string.Empty;
            BaseModel.AverageAnswerTimeIndicator = string.Empty;

            BaseModel.OnPropertyChangedOnlyForStatistics();
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Return the answer number for the given answer
        /// </summary>
        /// <param name="answer">The answer that should present and have a number</param>
        /// <exception cref="ArgumentOutOfRangeException">Number for answer not found</exception>
        /// <returns>The number of the anwser</returns>
        internal byte GetAnswerNumber(in TestBaseModel answer)
        {
            for(byte answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
            {
                if(BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber)?.Equals(answer) != true)
                {
                    continue;
                }

                return answerNumber;
            }

            throw new ArgumentOutOfRangeException(
                nameof(answer),
                answer,
                "Number for answer not found");
        }

        /// <summary>
        /// Return the hint for the given answer, based on the current ask sign
        /// </summary>
        /// <param name="answer">The answer for the hint</param>
        /// <returns>The hint for the given answer, based on the current ask sign</returns>
        /// <exception cref="ArgumentOutOfRangeException">Test type not supported</exception>
        internal string GetAnswerHintBasedOnAskSign(in TestBaseModel answer)
        {
            switch(BaseModel.SelectedTestType)
            {
                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji:
                    return answer.Roomaji;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    return answer.Hiragana;

                case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    return answer.Katakana;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(BaseModel.SelectedTestType),
                        BaseModel.SelectedTestType,
                        "Test type not supported");
            }
        }

        /// <summary>
        /// Set the colors for all hint texts (above of the answer buttons)
        /// </summary>
        /// <param name="answerNumber">The answer number or button number</param>
        /// <param name="currentAnswer">The given answer of the user</param>
        internal void SetHintTextColors(in int answerNumber, in TestBaseModel currentAnswer)
        {
            Debug.Assert(answerNumber is >= 0 and <= 9, $"[{nameof(answerNumber)}] must be in range of 0 to 9");

            if(BaseModel.SelectedHintShowType == HintShowType.None)
            {
                return;
            }

            if(BaseModel.HintTextColor.ElementAtOrDefault(answerNumber) == default)
            {
                return;
            }

            var possibleAnswer = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
            if(possibleAnswer == null)
            {
                return;
            }

            if(BaseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnMarkedAnswers)
            && BaseModel.AnswerButtonColor[answerNumber] == ColorHelper.MarkedColor
            && !possibleAnswer.Equals(BaseModel.CurrentTest)
            && !possibleAnswer.Equals(currentAnswer))
            {
                BaseModel.HintTextColor[answerNumber] = ColorHelper.HintTextColor;
            }

            if(BaseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnCorrectAnswer)
            && possibleAnswer.Equals(BaseModel.CurrentTest))
            {
                BaseModel.HintTextColor[answerNumber] = ColorHelper.HintTextColor;
            }

            if(BaseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnWrongAnswer)
            && !possibleAnswer.Equals(BaseModel.CurrentTest)
            && possibleAnswer.Equals(currentAnswer))
            {
                BaseModel.HintTextColor[answerNumber] = ColorHelper.HintTextColor;
            }

            if(BaseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnOtherAnswers)
            && BaseModel.AnswerButtonColor[answerNumber] != ColorHelper.MarkedColor
            && !possibleAnswer.Equals(BaseModel.CurrentTest)
            && !possibleAnswer.Equals(currentAnswer))
            {
                BaseModel.HintTextColor[answerNumber] = ColorHelper.HintTextColor;
            }
        }

        #endregion Internal Methods
    }
}
