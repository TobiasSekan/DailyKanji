using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace DailyKanjiLogic.Mvvm.ViewModel
{
    public class MainBaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Data model that contain all static and changeable runtime data of the program logic
        /// </summary>
        public MainBaseModel BaseModel { get; private set; }

        #endregion Public Properties

        #region Public Constructor

        public MainBaseViewModel()
            => BaseModel = new MainBaseModel();

        #endregion Public Constructor

        #region Public Methods

        public void InitalizeBaseModel(in string baseColor, in string progressBarColor)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(baseColor), $"{nameof(baseColor)} can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), $"{nameof(progressBarColor)} can't be empty or null");

            if(BaseModel == null)
            {
                BaseModel = new MainBaseModel();
            }

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != BaseModel.AllTestsList?.Count())
            {
                BaseModel.AllTestsList = list.ToList();
            }

            BaseModel.Randomizer        = new Random();
            BaseModel.PossibleAnswers   = new Collection<TestBaseModel>();
            BaseModel.TestPool          = new Collection<TestBaseModel>();
            BaseModel.AnswerButtonColor = new ObservableCollection<string>();
            BaseModel.HintTextColor     = new ObservableCollection<string>();
            BaseModel.HighlightTimer    = new ManualResetEvent(false);
            BaseModel.ProgressBarColor  = progressBarColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                BaseModel.AnswerButtonColor.Add(baseColor);
                BaseModel.HintTextColor.Add(baseColor);
            }

            BuildTestPool();
            ChooseNewSign(GetRandomKanaTest());
        }

        /// <summary>
        /// Reorder all tests by it own correct and wrong counters
        /// </summary>
        public void OrderAllTests()
            => BaseModel.AllTestsList
                = BaseModel.AllTestsList
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ChooseNewSign(TestBaseModel newTest)
        {
            Debug.Assert(newTest != null, $"{nameof(newTest)} can't be null");

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
                    var random = BaseModel.Randomizer.Next(0, 3);
                    switch(random)
                    {
                        case 0:
                            BaseModel.CurrentAskSign = BaseModel.CurrentTest.Hiragana;
                            break;

                        case 1:
                            BaseModel.CurrentAskSign = BaseModel.CurrentTest.Katakana;
                            break;

                        case 2:
                            BaseModel.CurrentAskSign = BaseModel.CurrentTest.Roomaji;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(random), $"[{random}] is not between 0 and 2");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        /// <summary>
        /// Return a random kana test and avoid that the test is the same as the current selected test
        /// </summary>
        /// <returns>A kana test</returns>
        public TestBaseModel GetRandomKanaTest()
        {
            var newTest = BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));

            if(BaseModel.CurrentTest == null)
            {
                return newTest;
            }

            while(newTest.Roomaji == BaseModel.CurrentTest.Roomaji)
            {
                newTest = BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));
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
                BaseModel.CurrentTest
            };

            var allAnswerList = BaseModel.SimilarAnswers
                ? KanaHelper.GetSimilarKana(BaseModel.TestPool.Distinct(), BaseModel.CurrentTest, BaseModel.CurrentTest.AnswerType).ToList()
                : BaseModel.TestPool.Distinct().ToList();

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

                var anyAnswer = GetRandomKanaTest();
                if(possibleAnswers.Contains(anyAnswer))
                {
                    // don't add test twice
                    continue;
                }

                possibleAnswers.Add(anyAnswer);
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetAnswerText(in TestBaseModel answer, AnswerType answerType)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");

            answer.AnswerType = answerType;

            var possibleAnswer = BaseModel.PossibleAnswers[GetAnswerNumber(answer)];

            switch(answerType)
            {
                case AnswerType.Roomaji:
                    return possibleAnswer.Roomaji;

                case AnswerType.Hiragana:
                    return possibleAnswer.Hiragana;

                case AnswerType.Katakana:
                    return possibleAnswer.Katakana;

                default:
                    throw new ArgumentOutOfRangeException(nameof(answerType), "Answer type not supported");
            }
        }

        /// <summary>
        /// Return a hint for a answer, based on the selected <see cref="HintType"/> and selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer that should have a hint</param>
        /// <returns>A hint for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetAnswerHint(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");

            switch(BaseModel.SelectedHintType)
            {
                case HintType.AlwaysInRoomaji:
                    return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Roomaji;

                case HintType.AlwaysInHiragana:
                    return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Hiragana;

                case HintType.AlwaysInKatakana:
                    return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Katakana;

                case HintType.BasedOnAskSign:
                    switch(BaseModel.SelectedTestType)
                    {
                        case TestType.RoomajiToHiraganaOrKatakana:
                        case TestType.RoomajiToHiragana:
                        case TestType.RoomajiToKatakana:
                        case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Roomaji:
                            return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Roomaji;

                        case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                        case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                        case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Hiragana:
                        case TestType.HiraganaToRoomaji:
                        case TestType.HiraganaToKatakana:
                            return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Hiragana;

                        case TestType.HiraganaOrKatakanaToRoomaji when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                        case TestType.HiraganaToKatakanaOrKatakanaToHiragana when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                        case TestType.AllToAll when BaseModel.CurrentAskSign == BaseModel.CurrentTest.Katakana:
                        case TestType.KatakanaToRoomaji:
                        case TestType.KatakanaToHiragana:
                            return BaseModel.PossibleAnswers[GetAnswerNumber(answer)].Katakana;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedHintType), "Hint type not supported");
            }
        }

        /// <summary>
        /// Count the result of a test, based on the given answer and the selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer of a test</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CountAnswerResult(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");

            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
            {
                if(answer.WrongnessCounter > 0)
                {
                    answer.WrongnessCounter--;
                }
                else
                {
                    answer.WrongnessCounter++;
                }
            }

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
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType),
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
                test.WrongnessCounter = 0;

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
        public bool IsRoomaji(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Roomaji == signToTest);

        /// <summary>
        /// Test if the given sign is a Hiragana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Hiragana, otherwise <see langword="false"/></returns>
        public bool IsHiragana(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Hiragana == signToTest);

        /// <summary>
        /// Test if the given sign is a Katakana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Katakana, otherwise <see langword="false"/></returns>
        public bool IsKatakana(string signToTest)
            => BaseModel.AllTestsList.Any(found => found.Katakana == signToTest);

        /// <summary>
        /// Try to load all settings
        /// </summary>
        /// <param name="path">The path to the setting file</param>
        /// <param name="exception">The possible thrown exception until the setting file is load</param>
        /// <returns><see langword="true"/> if the setting file could be load, otherwise <see langword="false"/></returns>
        public bool TryLoadSettings(in string path, out Exception? exception)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                exception = new ArgumentException("Settings path is empty.", nameof(path));
                return false;
            }

            if(!File.Exists(path))
            {
                exception = new FileNotFoundException("Settings file not found.", path);
                return false;
            }

            try
            {
                BaseModel = JsonHelper.ReadJson<MainBaseModel>(path);
                exception = null;
                return true;
            }
            catch(Exception thrownException)
            {
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
        /// <param name="normalColor">The base color for all elements</param>
        /// <param name="progressBarColor">The base color for the progress bar</param>
        public void SetNormalColors(in string normalColor, in string progressBarColor)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(normalColor), $"{nameof(normalColor)} can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), $"{nameof(progressBarColor)} can't be empty or null");

            BaseModel.CurrentAskSignColor = normalColor;
            BaseModel.ProgressBarColor    = progressBarColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                BaseModel.AnswerButtonColor[answerNumber] = normalColor;
                BaseModel.HintTextColor[answerNumber]     = normalColor;
            }
        }

        /// <summary>
        /// Set the highlight colors for all elements
        /// </summary>
        /// <param name="answer">The answers of the current test</param>
        /// <param name="correctColor">The color string for the correct element</param>
        /// <param name="errorColor">The color string for the elements</param>
        /// <param name="noneSelectedColor">The color string for none selected elements</param>
        /// <param name="hintColor">The color string for the hint elements</param>
        public void SetHighlightColors(in TestBaseModel answer, in string correctColor, in string errorColor, in string noneSelectedColor, in string hintColor)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(correctColor), $"{nameof(correctColor)} can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(errorColor), $"{nameof(errorColor)} can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(noneSelectedColor), $"{nameof(noneSelectedColor)} can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(hintColor), $"{nameof(hintColor)} can't be empty or null");

            BaseModel.CurrentAskSignColor = errorColor;
            BaseModel.ProgressBarColor    = errorColor;

            for(var answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
            {
                BaseModel.AnswerButtonColor[answerNumber] = BaseModel.PossibleAnswers[answerNumber].Roomaji == BaseModel.CurrentTest.Roomaji
                    ? correctColor
                    : BaseModel.PossibleAnswers[answerNumber].Roomaji == answer.Roomaji
                        ? errorColor
                        : noneSelectedColor;

                if(BaseModel.ShowHints)
                {
                    BaseModel.HintTextColor[answerNumber] = hintColor;
                }
            }
        }

        /// <summary>
        /// Set or remove the highlight color for one answer (when highlight color is set the color will be removed)
        /// </summary>
        /// <param name="answer">The answer to highlight</param>
        /// <param name="highlightColor">The color string for the answer to highlight</param>
        /// <param name="normalColor">The color string for the answer when it is not highlight</param>
        public void SetOrRemoveHighlightColorToOneAnswer(in TestBaseModel answer, in string highlightColor, in string normalColor)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(highlightColor), $"{nameof(highlightColor)} can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(normalColor), $"{nameof(normalColor)} can't be empty or null");

            var answerNumber = GetAnswerNumber(answer);

            BaseModel.AnswerButtonColor[answerNumber] = BaseModel.AnswerButtonColor[answerNumber] != highlightColor
                ? highlightColor
                : normalColor;
        }

        /// <summary>
        /// Check the given answer and count the result
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <returns><see langword="true"/> if the answer was correct, otherwise <see langword="false"/></returns>
        public bool CheckAndCountAnswer(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");

            BaseModel.IgnoreInput  = true;
            BaseModel.PreviousTest = BaseModel.CurrentTest;

            CountAnswerResult(answer);

            return answer.Roomaji == BaseModel.CurrentTest.Roomaji;
        }

        /// <summary>
        /// Count one correct or wrong result for a Hiragana question, based on the given answer
        /// </summary>
        /// <param name="answer">The answer for the counting</param>
        public void CountWrongOrCorrectHiragana(in TestBaseModel answer)
        {
            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
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
            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                    throw new ArgumentOutOfRangeException(nameof(BaseModel.SelectedTestType), "Test type not supported");
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Return the answer number for the given answer
        /// </summary>
        /// <param name="answer">The answer that should present and have a number</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal int GetAnswerNumber(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, $"{nameof(answer)} can't be null");

            for(var answerNumber = 0; answerNumber < BaseModel.MaximumAnswers; answerNumber++)
            {
                if(BaseModel.PossibleAnswers[answerNumber].Roomaji != answer.Roomaji)
                {
                    continue;
                }

                return answerNumber;
            }

            throw new ArgumentOutOfRangeException(nameof(answer), "Number for answer not found");
        }

        #endregion Internal Methods
    }
}
