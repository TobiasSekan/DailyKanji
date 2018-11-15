using DailyKanji.Enumerations;
using DailyKanji.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Media;

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper
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
        /// The current colour of all answer buttons
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Brush> AnswerButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current colour of all answer buttons
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Brush> HintTextColor
        {
            get => _hintTextColor;
            set
            {
                _hintTextColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current colour of the ask sign
        /// </summary>
        [JsonIgnore]
        public Brush CurrentAskSignColor
        {
            get => _currentAskSignColor;
            set
            {
                _currentAskSignColor = value;
                OnPropertyChanged();
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
            }
        }

        /// <summary>
        /// The count of maximum answers
        /// </summary>
        public byte MaximumAnswer
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
        /// The timeout for highlight a wrong answered question
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
        /// The maximum answer time in milliseconds
        /// </summary>
        public double MaximumAnswerTime
        {
            get => _maximumAnswerTime;
            set
            {
                _maximumAnswerTime = value;
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
        /// The color for the progress bar (running answer time)
        /// </summary>
        [JsonIgnore]
        public Brush ProgressBarColor
        {
            get => _progressBarColor;
            set
            {
                _progressBarColor = value;
                OnPropertyChanged();
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
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.WrongHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.WrongKatakanaCount}";
                }

                return string.Empty;
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
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.CorrectHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.CorrectKatakanaCount}";
                }

                return string.Empty;
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
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaOrHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.RoomajiToHiraganaOrKatakana when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Indicate that the user can go to the previous test
        /// </summary>
        [JsonIgnore]
        public bool CanGoToLastTest
            => PreviousTest != null;

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// A list with possible answers
        /// </summary>
        internal IList<TestBaseModel> PossibleAnswers { get; set; }

        /// <summary>
        /// Global random generator
        /// </summary>
        internal Random Randomizer { get; set; }

        /// <summary>
        /// Indicate that the current input (mouse and keyboard) will ignore and no processed
        /// </summary>
        internal bool IgnoreInput { get; set; }

        /// <summary>
        /// The time stamp when the test was start
        /// </summary>
        internal DateTime TestStartTime
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
        internal TestBaseModel PreviousTest { get; set; }

        internal Timer TestTimer { get; set; }

        #endregion Internal Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="CurrentTest"/>
        /// </summary>
        private TestBaseModel _currentTest;

        /// <summary>
        /// Backing-field for <see cref="AnswerButtonColor"/>
        /// </summary>
        private ObservableCollection<Brush> _buttonColor;

        /// <summary>
        /// Backing-field for <see cref="HintTextColor"/>
        /// </summary>
        private ObservableCollection<Brush> _hintTextColor;

        /// <summary>
        /// Backing-field for <see cref="CurrentAskSignColor"/>
        /// </summary>
        private Brush _currentAskSignColor;

        /// <summary>
        /// Backing-field for <see cref="CurrentAskSign"/>
        /// </summary>
        private string _currentAskSign;

        /// <summary>
        /// Backing-field for <see cref="MaximumAnswer"/>
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
        /// Backing-field for <see cref="MaximumAnswer"/>
        /// </summary>
        private double _maximumAnswerTime;

        /// <summary>
        /// Backing-field for <see cref="CurrentAnswerTime"/>
        /// </summary>
        private double _currentAnswerTime;

        /// <summary>
        /// Backing-field for <see cref="ProgressBarColor"/>
        /// </summary>
        private Brush _progressBarColor;

        #endregion Private Backing-Fields

        #region Public Constructors

        public MainModel()
        {
            MaximumAnswerTime  = 10_000;                                // 15 seconds
            ErrorTimeout       = 3_000;                                 //  3 seconds

            MaximumAnswer      = 7;

            SelectedTestType   = TestType.HiraganaOrKatakanaToRoomaji;

            ShowHints          = true;
            SimilarAnswers     = true;
        }

        #endregion Public Constructors
    }
}
