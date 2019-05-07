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
            Debug.Assert(!string.IsNullOrWhiteSpace(baseColor), "base color can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), "Progress bar color can't be null");

            if(BaseModel == null)
            {
                BaseModel = new MainBaseModel();
            }

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != BaseModel.AllTestsList?.Count())
            {
                BaseModel.AllTestsList = list.ToList();
            }

            BaseModel.Randomizer          = new Random();
            BaseModel.PossibleAnswers     = new Collection<TestBaseModel>();
            BaseModel.TestPool            = new Collection<TestBaseModel>();
            BaseModel.AnswerButtonColor   = new ObservableCollection<string>();
            BaseModel.HintTextColor       = new ObservableCollection<string>();
            BaseModel.HighlightTimer = new ManualResetEvent(false);
            BaseModel.ProgressBarColor    = progressBarColor;

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
            Debug.Assert(newTest != null, "Test model for choose sign can't be null");

            if(BaseModel.CurrentTest != null)
            {
                while(newTest?.Roomaji == BaseModel.CurrentTest.Roomaji)
                {
                    newTest = GetRandomKanaTest();
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
        /// Return a random kana test
        /// </summary>
        /// <returns>A kana test</returns>
        public TestBaseModel GetRandomKanaTest()
            => BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        public void ChooseNewPossibleAnswers()
        {
            var firstTestCharacter  = BaseModel.CurrentTest.Roomaji.FirstOrDefault();
            var secondTestCharacter = BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(1);
            var thirdTestCharacter  = BaseModel.CurrentTest.Roomaji.ElementAtOrDefault(2);

            var tryAddCount = 0;
            var list        = new ObservableCollection<TestBaseModel>
            {
                BaseModel.CurrentTest
            };

            while(list.Count < BaseModel.MaximumAnswers)
            {
                var possibleAnswer = GetRandomKanaTest();
                Debug.Assert(possibleAnswer != null, "Random kana test is null");

                if((tryAddCount < 50) && list.Any(found => found.Roomaji == possibleAnswer.Roomaji))
                {
                    tryAddCount++;
                    continue;
                }

                if(!BaseModel.SimilarAnswers)
                {
                    list.Add(possibleAnswer);
                    continue;
                }

                if((tryAddCount < 50)
                && !possibleAnswer.Roomaji.Contains(firstTestCharacter)
                && !possibleAnswer.Roomaji.Contains(secondTestCharacter)
                && !possibleAnswer.Roomaji.Contains(thirdTestCharacter))
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
            Debug.Assert(answerNumber < 10, $"Answer number must between 0 and 9, but it was [{answerNumber}]");

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
            Debug.Assert(answerNumber < 10, $"Answer number must between 0 and 9, but it was [{answerNumber}]");

            switch(BaseModel.SelectedHintType)
            {
                case HintType.AlwaysInRoomaji:
                    return BaseModel.PossibleAnswers[answerNumber].Roomaji;

                case HintType.AlwaysInHiragana:
                    return BaseModel.PossibleAnswers[answerNumber].Hiragana;

                case HintType.AlwaysInKatakana:
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CountAnswerResult(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, "Answer can't be null for counting answer result");

            var answerTime = DateTime.UtcNow - BaseModel.TestStartTime;

            if(answer.Roomaji == BaseModel.CurrentTest.Roomaji)
            {
                if(answer.WrongnessCounter > 0)
                {
                    answer.WrongnessCounter--;
                }

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

            answer.WrongnessCounter++;

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
        public bool TryLoadSettings(in string path, out Exception exception)
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
        public bool TrySaveSettings(in string path, out Exception exception)
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
        /// <param name="baseColor">The base color for all elements</param>
        /// <param name="progressBarColor">The base color for the progress bar</param>
        public void SetNormalColors(in string baseColor, in string progressBarColor)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(baseColor), "Base color can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), "Progress bar color can't be null");

            BaseModel.CurrentAskSignColor = baseColor;
            BaseModel.ProgressBarColor    = progressBarColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                BaseModel.AnswerButtonColor[answerNumber] = baseColor;
                BaseModel.HintTextColor[answerNumber]     = baseColor;
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
            Debug.Assert(answer != null, "Answer can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(correctColor), "Correction color can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(errorColor), "Error color can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(noneSelectedColor), "None selected color can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(hintColor), "Hint bar color can't be null");

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
        /// Check the given answer and count the result
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <returns><see langword="true"/> if the answer was correct, otherwise <see langword="false"/></returns>
        public bool CheckAndCountAnswer(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, "Answer can't be null for answer check");

            BaseModel.IgnoreInput = true;

            if(answer is null)
            {
                answer.WrongnessCounter++;
                return false;
            }

            BaseModel.PreviousTest = BaseModel.CurrentTest;

            CountAnswerResult(answer);

            return answer.Roomaji == BaseModel.CurrentTest.Roomaji;
        }
    }

    #endregion Public Methods
}
