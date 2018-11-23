﻿using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyKanjiLogic.Mvvm.Model
{
    public class MainBaseModel : PropertyChangedHelper
    {
        #region Public Properties

        /// <summary>
        /// The current sign quest
        /// </summary>
        [JsonIgnore]
        public TestBaseModel CurrentTest
        {
            get => _currentTest;
            set
            {
                _currentTest = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllTestsList));
            }
        }

        /// <summary>
        /// The current ask sign
        /// </summary>
        [JsonIgnore]
        public string CurrentAskSign
        {
            get => _currentAskSign;
            set
            {
                _currentAskSign = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WrongCount));
                OnPropertyChanged(nameof(CorrectCount));
                OnPropertyChanged(nameof(AverageAnswerTime));
                OnPropertyChanged(nameof(WrongAnswerCountString));
                OnPropertyChanged(nameof(RightAnswerCountString));
                OnPropertyChanged(nameof(CurrentRateText));
            }
        }

        /// <summary>
        /// The count of maximum answers
        /// </summary>
        public byte MaximumAnswers
        {
            get => _maximumAnswer;
            set
            {
                _maximumAnswer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that only similar answers will be shown as possible answers
        /// </summary>
        public bool SimilarAnswers
        {
            get => _similarAnswers;
            set
            {
                _similarAnswers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List that contains the complete test pool
        /// (one of this test will be ask each test round)
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<TestBaseModel> TestPool
        {
            get => _testPool;
            set
            {
                _testPool = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The timeout for highlight a wrong answered question in milliseconds
        /// </summary>
        public int ErrorTimeout
        {
            get => _errorTimeout;
            set
            {
                _errorTimeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current selected test type
        /// </summary>
        public TestType SelectedTestType
        {
            get => _selectedTestType;
            set
            {
                _selectedTestType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TestTypeString));
            }
        }

        /// <summary>
        /// List with all possible tests
        /// </summary>
        public ICollection<TestBaseModel> AllTestsList
        {
            get => _allTestsList;
            set
            {
                _allTestsList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that hints on wrong answers will be shown
        /// </summary>
        public bool ShowHints
        {
            get => _showHints;
            set
            {
                _showHints = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The maximum answer timeout in milliseconds
        /// </summary>
        public double MaximumAnswerTimeout
        {
            get => _maximumAnswerTime;
            set
            {
                _maximumAnswerTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the shortcuts for the answers will be shown
        /// </summary>
        public bool ShowAnswerShortcuts
        {
            get => _showAnswerShortcuts;
            set
            {
                _showAnswerShortcuts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the running timer for a answer will be shown
        /// </summary>
        public bool ShowRunningAnswerTimer
        {
            get => _showRunningAnswerTimer;
            set
            {
                _showRunningAnswerTimer = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        /// <summary>
        /// The current answer time in milliseconds
        /// </summary>
        public double CurrentAnswerTime
        {
            get => _currentAnswerTime;
            set
            {
                _currentAnswerTime = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public string WrongAnswerCountString
            => $"H: {AllTestsList.Sum(found => found.WrongHiraganaCount)}"
             + $" K: {AllTestsList.Sum(found => found.WrongKatakanaCount)}";

        [JsonIgnore]
        public string RightAnswerCountString
            => $"H: {AllTestsList.Sum(found => found.CorrectHiraganaCount)}"
             + $" K: {AllTestsList.Sum(found => found.CorrectKatakanaCount)}";

        [JsonIgnore]
        public string TestTypeString
        {
            get
            {
                switch(SelectedTestType)
                {
                    case TestType.HiraganaOrKatakanaToRoomaji:
                        return "H / K => R";

                    case TestType.HiraganaToRoomaji:
                        return "H => R";

                    case TestType.KatakanaToRoomaji:
                        return "K => R";

                    case TestType.RoomajiToHiraganaOrKatakana:
                        return "R => H / K";

                    case TestType.RoomajiToHiragana:
                        return "R => H";

                    case TestType.RoomajiToKatakana:
                        return "R => K";

                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana:
                        return "H => K / K => H";

                    case TestType.HiraganaToKatakana:
                        return "H => K";

                    case TestType.KatakanaToHiragana:
                        return "K => H";
                }

                return "unknown";
            }
        }

        /// <summary>
        /// Return the current rate in percent
        /// </summary>
        [JsonIgnore]
        public string CurrentRateText
        {
            get
            {
                var wrongAnswerCount = AllTestsList.Sum(found => found.WrongHiraganaCount + found.WrongKatakanaCount);
                var rightAnswerCount = AllTestsList.Sum(found => found.CorrectHiraganaCount + found.CorrectKatakanaCount);

                return wrongAnswerCount != 0
                    ? $"{Math.Round(100.0 / (wrongAnswerCount + rightAnswerCount) * rightAnswerCount, 2)}%"
                    : "100%";
            }
        }

        [JsonIgnore]
        public string WrongCount
        {
            get
            {
                if(CurrentTest == null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.WrongHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.WrongKatakanaCount}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.CorrectHiraganaCount} - K: {CurrentTest.CorrectKatakanaCount}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        [JsonIgnore]
        public string CorrectCount
        {
            get
            {
                if(CurrentTest == null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.CorrectHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.CorrectKatakanaCount}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.CorrectHiraganaCount} - K: {CurrentTest.CorrectKatakanaCount}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        [JsonIgnore]
        public string AverageAnswerTime
        {
            get
            {
                if(CurrentTest == null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff} - K: {CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        /// <summary>
        /// Indicate that the user can go to the previous test
        /// </summary>
        [JsonIgnore]
        public bool CanGoToLastTest
            => PreviousTest != null;


        /// <summary>
        /// A list with possible answers
        /// </summary>
        [JsonIgnore]
        public IList<TestBaseModel> PossibleAnswers { get; set; }

        /// <summary>
        /// Global random generator
        /// </summary>
        [JsonIgnore]
        public Random Randomizer { get; set; }

        /// <summary>
        /// Indicate that the current input (mouse and keyboard) will ignore and no processed
        /// </summary>
        [JsonIgnore]
        public bool IgnoreInput { get; set; }

        /// <summary>
        /// The time stamp when the test was start
        /// </summary>
        [JsonIgnore]
        public DateTime TestStartTime
        {
            get => _testStartTime;
            set
            {
                _testStartTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoToLastTest));
            }
        }

        /// <summary>
        /// The previous tests
        /// </summary>
        [JsonIgnore]
        public TestBaseModel PreviousTest { get; set; }

        /// <summary>
        /// Return the library version
        /// </summary>
        [JsonIgnore]
        public Version GetVersion
            => AssemblyHelper.GetAssemblyVersion(this);

        #endregion Public Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="CurrentTest"/>
        /// </summary>
        private TestBaseModel _currentTest;

        /// <summary>
        /// Backing-field for <see cref="CurrentAskSign"/>
        /// </summary>
        private string _currentAskSign;

        /// <summary>
        /// Backing-field for <see cref="MaximumAnswers"/>
        /// </summary>
        private byte _maximumAnswer;

        /// <summary>
        /// Backing-field for <see cref="SimilarAnswers"/>
        /// </summary>
        private bool _similarAnswers;

        /// <summary>
        /// Backing-field for <see cref="TestPool"/>
        /// </summary>
        private IReadOnlyCollection<TestBaseModel> _testPool;

        /// <summary>
        /// Backing-field for <see cref="ErrorTimeout"/>
        /// </summary>
        private int _errorTimeout;

        /// <summary>
        /// Backing-field for <see cref="SelectedTestType"/>
        /// </summary>
        private TestType _selectedTestType;

        /// <summary>
        /// Backing-field for <see cref="AllTestsList"/>
        /// </summary>
        private ICollection<TestBaseModel> _allTestsList;

        /// <summary>
        /// Backing-field for <see cref="ShowHints"/>
        /// </summary>
        private bool _showHints;

        /// <summary>
        /// Backing-filed for <see cref="TestStartTime"/>
        /// </summary>
        private DateTime _testStartTime;

        /// <summary>
        /// Backing-field for <see cref="MaximumAnswers"/>
        /// </summary>
        private double _maximumAnswerTime;

        /// <summary>
        /// Backing-field for <see cref="CurrentAnswerTime"/>
        /// </summary>
        private double _currentAnswerTime;

        /// <summary>
        /// Backing-filed for <see cref="ShowAnswerShortcuts"/>
        /// </summary>
        private bool _showAnswerShortcuts;

        /// <summary>
        /// Backing-field for <see cref="ShowRunningAnswerTimer"/>
        /// </summary>
        private bool _showRunningAnswerTimer;

        #endregion Private Backing-Fields

        #region Public Constructors

        public MainBaseModel()
        {
            MaximumAnswerTimeout   = 10_000;
            ErrorTimeout           = 3_000;
            MaximumAnswers         = 7;
            SelectedTestType       = TestType.HiraganaOrKatakanaToRoomaji;
            ShowHints              = true;
            ShowAnswerShortcuts    = true;
            ShowRunningAnswerTimer = true;
            SimilarAnswers         = true;
        }

        #endregion Public Constructors
    }
}