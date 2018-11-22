using DailyKanjiLogic.Enumerations;
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
        /// Build the test pool (wrong answered tests will add multiple)
        /// </summary>
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
        /// Return a new random test
        /// </summary>
        /// <param name="onlyOneRoomajiCharacter">(Optional) Indicate that only a test that have a roomaji character with length one will return</param>
        /// <returns>A test</returns>
        public TestBaseModel GetRandomTest(bool onlyOneRoomajiCharacter = false)
            => onlyOneRoomajiCharacter
                ? BaseModel.TestPool.Where(found => found.Roomaji.Length == 1)
                                    .ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count))
                : BaseModel.TestPool.ElementAtOrDefault(BaseModel.Randomizer.Next(0, BaseModel.TestPool.Count));

        /// <summary>
        /// Choose new possible answers for the current ask sign
        /// </summary>
        public void ChooseNewPossibleAnswers()
        {
            // .Net Standard 1.0 doesn't support LINQ chain methods on strings
            var firstTestChrachter  = BaseModel.CurrentTest.Roomaji.ToCharArray().FirstOrDefault().ToString();
            var secondTestChrachter = BaseModel.CurrentTest.Roomaji.ToCharArray().ElementAtOrDefault(1).ToString();
            var ThirdTestChrachter  = BaseModel.CurrentTest.Roomaji.ToCharArray().ElementAtOrDefault(2).ToString();

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
        /// Rest the complete statistic
        /// </summary>
        public void ResetCompleteStatistic()
        {
            foreach(var test in BaseModel.AllTestsList)
            {
                test.CorrectHiraganaCount = 0;
                test.CorrectKatakanaCount = 0;
                test.WrongHiraganaCount = 0;
                test.WrongKatakanaCount = 0;
                test.CompleteAnswerTimeForHiragana = new TimeSpan();
                test.CompleteAnswerTimeForKatakana = new TimeSpan();
            }
        }
    }

    #endregion Public Methods
}
