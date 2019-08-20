using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private MainBaseModel _baseModel { get; }

        #endregion Public Properties

        #region Public Constructor

        public MainBaseViewModel(in MainBaseModel baseModel, in string baseColor, in string progressBarColor)
        {
            _baseModel = baseModel;

            Debug.Assert(!string.IsNullOrWhiteSpace(baseColor), $"MainBaseViewModel: [{nameof(baseColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), $"MainBaseViewModel: [{nameof(progressBarColor)}] can't be empty or null");

            var list = KanaHelper.GetKanaList();
            if(list?.Count() != _baseModel.AllTestsList?.Count())
            {
                _baseModel.AllTestsList = list.ToList();
            }

            _baseModel.Randomizer        = new Random();
            _baseModel.PossibleAnswers   = new Collection<TestBaseModel>();
            _baseModel.TestPool          = new Collection<TestBaseModel>();
            _baseModel.AnswerButtonColor = new ObservableCollection<string>();
            _baseModel.HintTextColor     = new ObservableCollection<string>();
            _baseModel.HighlightTimer    = new ManualResetEvent(false);
            _baseModel.ProgressBarColor  = progressBarColor;

            var answerButtonColorList = new List<string>(10);
            var hintTextColorList     = new List<string>(10);

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                answerButtonColorList.Add(baseColor);
                hintTextColorList.Add(baseColor);
            }

            _baseModel.AnswerButtonColor = answerButtonColorList;
            _baseModel.HintTextColor     = hintTextColorList;

            BuildTestPool();

            ChooseNewSign(GetRandomKanaTest());
        }

        #endregion Public Constructor

        #region Public Methods

        /// <summary>
        /// Reorder all tests by it own correct and wrong counters
        /// </summary>
        public void OrderAllTests()
            => _baseModel.AllTestsList = _baseModel.AllTestsList
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

            foreach(var test in _baseModel.AllTestsList)
            {
                if(!_baseModel.SelectedKanaType.HasFlag(test.Type))
                {
                    continue;
                }

                var repeatCounter = test.WrongnessCounter * 10;

                for(var count = 0; count <= repeatCounter; count++)
                {
                    testPool.Add(test);
                }
            }

            _baseModel.TestPool = testPool;
        }

        /// <summary>
        /// Choose a new sign for a new ask
        /// </summary>
        /// <param name="newTest">The test for the new sign</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ChooseNewSign(in TestBaseModel newTest)
        {
            _baseModel.CurrentTest = newTest;

            switch(_baseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana:
                    _baseModel.CurrentAskSign = _baseModel.Randomizer.Next(0, 2) == 0
                        ? _baseModel.CurrentTest.Hiragana
                        : _baseModel.CurrentTest.Katakana;
                    break;

                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    _baseModel.CurrentAskSign = _baseModel.CurrentTest.Hiragana;
                    break;

                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    _baseModel.CurrentAskSign = _baseModel.CurrentTest.Katakana;
                    break;

                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                    _baseModel.CurrentAskSign = _baseModel.CurrentTest.Roomaji;
                    break;

                case TestType.AllToAll:
                    var randomNumber = _baseModel.Randomizer.Next(0, 3);

                    _baseModel.CurrentAskSign = randomNumber switch
                    {
                        0 => _baseModel.CurrentTest.Hiragana,
                        1 => _baseModel.CurrentTest.Katakana,
                        2 => _baseModel.CurrentTest.Roomaji,
                        _ => throw new ArgumentOutOfRangeException(nameof(randomNumber),
                                                                   randomNumber,
                                                                   "ChooseNewSign: is not between 0 and 2"),
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_baseModel.SelectedTestType),
                                                          _baseModel.SelectedTestType,
                                                          "ChooseNewSign: Test type not supported");
            }
        }

        /// <summary>
        /// Return a random kana test and avoid that the test is the same as the current selected test
        /// </summary>
        /// <returns>A kana test</returns>
        public TestBaseModel GetRandomKanaTest()
        {
            var testPollCount = _baseModel.TestPool.Count();
            var newTest       = _baseModel.TestPool.ElementAtOrDefault(_baseModel.Randomizer.Next(0, testPollCount));

            if(_baseModel.CurrentTest is null)
            {
                return newTest;
            }

            while(newTest.Roomaji == _baseModel.CurrentTest.Roomaji)
            {
                newTest = _baseModel.TestPool.ElementAtOrDefault(_baseModel.Randomizer.Next(0, testPollCount));
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
                _baseModel.CurrentTest
            };

            var allAnswerList = _baseModel.SimilarAnswers
                ? KanaHelper.GetSimilarKana(_baseModel.TestPool.Distinct(), _baseModel.CurrentTest, _baseModel.CurrentTest.AnswerType).ToList()
                : _baseModel.TestPool.Distinct().ToList();

            allAnswerList.Remove(_baseModel.CurrentTest);
            allAnswerList.Shuffle();

            while(possibleAnswers.Count < _baseModel.MaximumAnswers)
            {
                if(allAnswerList.Count > 0)
                {
                    var possibleAnswer = allAnswerList.ElementAtOrDefault(_baseModel.Randomizer.Next(0, allAnswerList.Count));
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

            _baseModel.PossibleAnswers = possibleAnswers;
        }

        /// <summary>
        /// Return a text for a answer, based on the given <see cref="AnswerType"/>
        /// </summary>
        /// <param name="answer">The answer, that should have a text</param>
        /// <param name="answerType">The type of the answer</param>
        /// <returns>A text for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetAnswerText(in TestBaseModel answer, in AnswerType answerType)
        {
            answer.AnswerType = answerType;

            return answerType switch
            {
                AnswerType.Roomaji  => answer.Roomaji,
                AnswerType.Hiragana => answer.Hiragana,
                AnswerType.Katakana => answer.Katakana,
                _                   => throw new ArgumentOutOfRangeException(nameof(answerType), answerType, "Answer type not supported"),
            };
        }

        /// <summary>
        /// Return a hint for a answer, based on the selected <see cref="HintType"/> and selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer that should have a hint</param>
        /// <returns>A hint for a answer</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetAnswerHint(in TestBaseModel answer)
            => _baseModel.SelectedHintType switch
            {
                HintType.AlwaysInRoomaji => answer.Roomaji,
                HintType.AlwaysInHiragana => answer.Hiragana,
                HintType.AlwaysInKatakana => answer.Katakana,
                HintType.BasedOnAskSign => GetAnswerHintBasedOnAskSign(answer),
                _ => throw new ArgumentOutOfRangeException(nameof(_baseModel.SelectedHintType), _baseModel.SelectedHintType, "Hint type not supported"),
            };

        /// <summary>
        /// Count the result of a test, based on the given answer and the selected <see cref="TestType"/>
        /// </summary>
        /// <param name="answer">The answer of a test</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CountAnswerResult(in TestBaseModel answer)
        {
            switch(_baseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Hiragana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.RoomajiToHiragana:
                case TestType.HiraganaToKatakana:
                    CountWrongOrCorrectHiragana(answer);
                    return;

                case TestType.HiraganaOrKatakanaToRoomaji when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Katakana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Katakana:
                case TestType.KatakanaToRoomaji:
                case TestType.RoomajiToKatakana:
                case TestType.KatakanaToHiragana:
                    CountWrongOrCorrectKatakana(answer);
                    return;

                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Roomaji && answer.AnswerType == AnswerType.Unknown:
                case TestType.RoomajiToHiraganaOrKatakana when answer.AnswerType == AnswerType.Unknown:
                    CountWrongHiarganaOrKatakana();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_baseModel.SelectedTestType),
                                                         _baseModel,
                                                         "CountAnswerResult: Test type combination not supported\n"
                                                         + $"SelectedTestType:     [{_baseModel.SelectedTestType.ToString()}]\n"
                                                         + $"CurrentAskSign:       [{_baseModel.CurrentAskSign}]\n"
                                                         + $"CurrentTest.Katakana: [{_baseModel.CurrentTest.Katakana}]\n"
                                                         + $"CurrentTest.Hiragana: [{_baseModel.CurrentTest.Hiragana}]\n"
                                                         + $"answer.AnswerType:    [{answer.AnswerType.ToString()}]");
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
            foreach(var test in _baseModel.AllTestsList)
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

            return _baseModel.AllTestsList.Any(found => found.Roomaji == romajiToTest);
        }

        /// <summary>
        /// Test if the given sign is a Hiragana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Hiragana, otherwise <see langword="false"/></returns>
        public bool IsHiragana(in string signToTest)
        {
            var hiarganaToTest = signToTest;

            return _baseModel.AllTestsList.Any(found => found.Hiragana == hiarganaToTest);
        }

        /// <summary>
        /// Test if the given sign is a Katakana sign
        /// </summary>
        /// <param name="signToTest">The sign to test</param>
        /// <returns><see langword="true"/> if the given sign is a Katakana, otherwise <see langword="false"/></returns>
        public bool IsKatakana(in string signToTest)
        {
            var katakanaToTest = signToTest;

            return _baseModel.AllTestsList.Any(found => found.Katakana == katakanaToTest);
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
                exception = new ArgumentException("TryLoadSettings: Settings path is empty.", nameof(path));
                return false;
            }

            if(!File.Exists(path))
            {
                baseModel = new MainBaseModel();
                exception = new FileNotFoundException("TryLoadSettings: Settings file not found.", path);
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
                JsonHelper.WriteJson(path, _baseModel);
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
            Debug.Assert(!string.IsNullOrWhiteSpace(normalColor), $"SetNormalColors: [{nameof(normalColor)}] can't be null");
            Debug.Assert(!string.IsNullOrWhiteSpace(progressBarColor), $"SetNormalColors: [{nameof(progressBarColor)}] can't be empty or null");

            _baseModel.CurrentAskSignColor = normalColor;
            _baseModel.ProgressBarColor    = progressBarColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _baseModel.AnswerButtonColor[answerNumber] = normalColor;
                _baseModel.HintTextColor[answerNumber]     = normalColor;
            }

            _baseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Set the highlight colors for all elements
        /// </summary>
        /// <param name="answer">The answers of the current test</param>
        /// <param name="correctColor">The color string for the correct element</param>
        /// <param name="errorColor">The color string for the elements</param>
        /// <param name="markedColor">The color string for marked elements</param>
        /// <param name="hintColor">The color string for the hint elements</param>
        public void SetHighlightColors(in TestBaseModel answer,
                                       in string correctColor,
                                       in string errorColor,
                                       in string markedColor,
                                       in string hintColor)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(correctColor), $"SetHighlightColors: [{nameof(correctColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(errorColor), $"SetHighlightColors: [{nameof(errorColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(markedColor), $"SetHighlightColors: [{nameof(markedColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(hintColor), $"SetHighlightColors: [{nameof(hintColor)}] can't be empty or null");

            _baseModel.CurrentAskSignColor = errorColor;
            _baseModel.ProgressBarColor    = errorColor;

            for(var answerNumber = 0; answerNumber < _baseModel.MaximumAnswers; answerNumber++)
            {
                SetHintTextColors(answerNumber, answer, markedColor, hintColor);

                if(_baseModel.AnswerButtonColor.ElementAtOrDefault(answerNumber) == null)
                {
                    continue;
                }

                var possibleAnswer = _baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);

                if(possibleAnswer.Roomaji == _baseModel.CurrentTest.Roomaji)
                {
                    _baseModel.AnswerButtonColor[answerNumber] = correctColor;
                }
                else
                {
                    _baseModel.AnswerButtonColor[answerNumber] = possibleAnswer.Roomaji == answer.Roomaji
                        ? errorColor
                        : markedColor;
                }
            }

            _baseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Set or remove the highlight color for one answer (when highlight color is set the color will be removed)
        /// </summary>
        /// <param name="answer">The answer to highlight</param>
        /// <param name="highlightColor">The color string for the answer to highlight</param>
        /// <param name="normalColor">The color string for the answer when it is not highlight</param>
        public void SetOrRemoveHighlightColorToOneAnswer(in TestBaseModel answer, in string highlightColor, in string normalColor)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(highlightColor), $"SetOrRemoveHighlightColorToOneAnswer: [{nameof(highlightColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(normalColor), $"SetOrRemoveHighlightColorToOneAnswer: [{nameof(normalColor)}] can't be empty or null");

            var answerNumber = GetAnswerNumber(answer);

            _baseModel.AnswerButtonColor[answerNumber] = _baseModel.AnswerButtonColor[answerNumber] != highlightColor
                ? highlightColor
                : normalColor;

            _baseModel.OnPropertyChangedForAnswerButtonColors();
        }

        /// <summary>
        /// Check the given answer and count the result
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <returns><see langword="true"/> if the answer was correct, otherwise <see langword="false"/></returns>
        public bool CheckAndCountAnswer(in TestBaseModel answer)
        {
            _baseModel.IgnoreInput  = true;
            _baseModel.PreviousTest = _baseModel.CurrentTest;

            CountAnswerResult(answer);

            return answer.Roomaji == _baseModel.CurrentTest.Roomaji;
        }

        /// <summary>
        /// Count one correct or wrong result for a Hiragana question, based on the given answer
        /// </summary>
        /// <param name="answer">The answer for the counting</param>
        public void CountWrongOrCorrectHiragana(in TestBaseModel answer)
        {
            if(answer.Roomaji == _baseModel.CurrentTest.Roomaji)
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForCorrectHiragana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.CorrectHiraganaCount++;
            }
            else
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.WrongHiraganaCount++;
            }
        }

        /// <summary>
        /// Count one correct or wrong result for a Katakana question, based on the given answer
        /// </summary>
        /// <param name="answer">The answer for the counting</param>
        public void CountWrongOrCorrectKatakana(in TestBaseModel answer)
        {
            if(answer.Roomaji == _baseModel.CurrentTest.Roomaji)
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForCorrectKatakana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.CorrectKatakanaCount++;
            }
            else
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForWrongKatakana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.WrongKatakanaCount++;
            }
        }

        /// <summary>
        /// Count one wrong result for a unknown or skipped answer
        /// </summary>
        public void CountWrongHiarganaOrKatakana()
        {
            // When no answer is selected and the ask sign is in Roomaji,
            // we don't know which error counter we should increase. So we decide the coincidence
            if(_baseModel.Randomizer.Next(0, 2) == 0)
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForWrongHiragana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.WrongHiraganaCount++;
            }
            else
            {
                _baseModel.CurrentTest.CompleteAnswerTimeForWrongKatakana += _baseModel.AnswerTime;
                _baseModel.CurrentTest.WrongKatakanaCount++;
            }
        }

        /// <summary>
        /// Return the answer type for the current selected test, based on the current <see cref="BaseModel.SelectedTestType"/>
        /// </summary>
        /// <returns>The answer type for the current test</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AnswerType GetAnswerType()
        {
            switch(_baseModel.SelectedTestType)
            {
                case TestType.HiraganaOrKatakanaToRoomaji:
                case TestType.HiraganaToRoomaji:
                case TestType.KatakanaToRoomaji:
                    return AnswerType.Roomaji;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.RoomajiToHiragana:
                case TestType.KatakanaToHiragana:
                    return AnswerType.Hiragana;

                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign != _baseModel.CurrentTest.Katakana:
                case TestType.RoomajiToKatakana:
                case TestType.HiraganaToKatakana:
                    return AnswerType.Katakana;

                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Roomaji:
                    return (_baseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Hiragana : AnswerType.Katakana;

                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                    return (_baseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Katakana : AnswerType.Roomaji;

                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                    return (_baseModel.Randomizer.Next(0, 2) == 0) ? AnswerType.Hiragana : AnswerType.Roomaji;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_baseModel.SelectedTestType), _baseModel.SelectedTestType, "GetAnswerType: Test type not supported");
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
            var correctAfter  = _baseModel.CurrentTest.CorrectHiraganaCount + _baseModel.CurrentTest.CorrectKatakanaCount;

            var wrongBefore = test.WrongHiraganaCount + test.WrongKatakanaCount;
            var wrongAfter  = _baseModel.CurrentTest.WrongHiraganaCount + _baseModel.CurrentTest.WrongKatakanaCount;

            var answerTimeBefore = test.AverageAnswerTimeForHiragana + test.AverageAnswerTimeForKatakana;
            var answerTimeAfter  = _baseModel.CurrentTest.AverageAnswerTimeForHiragana + _baseModel.CurrentTest.AverageAnswerTimeForKatakana;

            if(correctBefore != correctAfter)
            {
                _baseModel.HighlightCorrectCounter = true;
                _baseModel.CorrectCountIndicator   = correctBefore < correctAfter ? "⇧" : "⇩";
            }

            if(wrongBefore != wrongAfter)
            {
                _baseModel.HighlightWrongCounter = true;
                _baseModel.WrongCountIndicator   = wrongBefore < wrongAfter ? "⇧" : "⇩";
            }

            if(answerTimeBefore != answerTimeAfter)
            {
                _baseModel.HighlightAnswerTime        = true;
                _baseModel.AverageAnswerTimeIndicator = answerTimeBefore < answerTimeAfter ? "⇧" : "⇩";
            }

            _baseModel.OnPropertyChangedOnlyForStatistics();
        }

        /// <summary>
        /// Reset all highlight properties
        /// </summary>
        public void ResetHighlight()
        {
            _baseModel.HighlightCorrectCounter = false;
            _baseModel.HighlightWrongCounter   = false;
            _baseModel.HighlightAnswerTime     = false;

            _baseModel.CorrectCountIndicator      = string.Empty;
            _baseModel.WrongCountIndicator        = string.Empty;
            _baseModel.AverageAnswerTimeIndicator = string.Empty;

            _baseModel.OnPropertyChangedOnlyForStatistics();
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
            for(var answerNumber = 0; answerNumber < _baseModel.MaximumAnswers; answerNumber++)
            {
                if(_baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber)?.Roomaji != answer.Roomaji)
                {
                    continue;
                }

                return answerNumber;
            }

            throw new ArgumentOutOfRangeException(nameof(answer), answer, "GetAnswerNumber: Number for answer not found");
        }

        /// <summary>
        /// Return the hint for the given answer, based on the current ask sign
        /// </summary>
        /// <param name="answer">The answer for the hint</param>
        /// <returns>The hint for the given answer, based on the current ask sign</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal string GetAnswerHintBasedOnAskSign(in TestBaseModel answer)
        {
            switch(_baseModel.SelectedTestType)
            {
                case TestType.RoomajiToHiraganaOrKatakana:
                case TestType.RoomajiToHiragana:
                case TestType.RoomajiToKatakana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Roomaji:
                    return answer.Roomaji;

                case TestType.HiraganaOrKatakanaToRoomaji when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Hiragana:
                case TestType.HiraganaToRoomaji:
                case TestType.HiraganaToKatakana:
                    return answer.Hiragana;

                case TestType.HiraganaOrKatakanaToRoomaji when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.HiraganaToKatakanaOrKatakanaToHiragana when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.AllToAll when _baseModel.CurrentAskSign == _baseModel.CurrentTest.Katakana:
                case TestType.KatakanaToRoomaji:
                case TestType.KatakanaToHiragana:
                    return answer.Katakana;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_baseModel.SelectedTestType),
                                                          _baseModel.SelectedTestType,
                                                          "GetAnswerHintBasedOnAskSign: Test type not supported");
            }
        }

        /// <summary>
        /// Set the colors for all hint texts (above of the answer buttons)
        /// </summary>
        /// <param name="answerNumber">The answer number or button number</param>
        /// <param name="answer">The given answer of the user</param>
        /// <param name="markedColor">The color string for marked elements</param>
        /// <param name="hintColor">The color string for the hint elements</param>
        internal void SetHintTextColors(in int answerNumber, in TestBaseModel answer, in string markedColor, in string hintColor)
        {
            Debug.Assert(answerNumber >= 0 && answerNumber <= 9, $"SetHintTextColors: [{nameof(answerNumber)}] must be in range of 0 to 9");
            Debug.Assert(!string.IsNullOrWhiteSpace(markedColor), $"SetHintTextColors: [{nameof(markedColor)}] can't be empty or null");
            Debug.Assert(!string.IsNullOrWhiteSpace(hintColor), $"SetHintTextColors: [{nameof(hintColor)}] can't be empty or null");

            if(_baseModel.SelectedHintShowType == HintShowType.ShowOnNoAnswers)
            {
                return;
            }

            if(_baseModel.HintTextColor.ElementAtOrDefault(answerNumber) == null)
            {
                return;
            }

            if(_baseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnMarkedAnswers)
            && _baseModel.AnswerButtonColor[answerNumber] == markedColor)
            {
                _baseModel.HintTextColor[answerNumber] = hintColor;
            }

            var possibleAnswer = _baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber);
            if(possibleAnswer == null)
            {
                return;
            }

            if(_baseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnCorrectAnswer)
            && possibleAnswer.Roomaji == _baseModel.CurrentTest.Roomaji)
            {
                _baseModel.HintTextColor[answerNumber] = hintColor;
            }

            if(_baseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnWrongAnswer)
            && possibleAnswer.Roomaji != _baseModel.CurrentTest.Roomaji
            && possibleAnswer.Roomaji == answer.Roomaji)
            {
                _baseModel.HintTextColor[answerNumber] = hintColor;
            }

            if(_baseModel.SelectedHintShowType.HasFlag(HintShowType.ShowOnOtherAnswers)
            && possibleAnswer.Roomaji != _baseModel.CurrentTest.Roomaji
            && possibleAnswer.Roomaji != answer.Roomaji
            && _baseModel.AnswerButtonColor[answerNumber] != markedColor)
            {
                _baseModel.HintTextColor[answerNumber] = hintColor;
            }
        }

        #endregion Internal Methods
    }
}
