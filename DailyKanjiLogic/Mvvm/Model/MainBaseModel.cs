using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DailyKanjiLogic.Mvvm.Model
{
    /// <summary>
    /// A data model that contain all data for the program logic and all Kanji data
    /// </summary>
    public class MainBaseModel : PropertyChangedHelper, IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Timer for the answer highlight, when a test was correct or wrong answered
        /// </summary>
        [JsonIgnore]
        public ManualResetEvent HighlightTimer { get; internal set; }

        /// <summary>
        /// The current colors of all answer buttons
        /// </summary>
        [JsonIgnore]
        public IList<string> AnswerButtonColor { get; set; }

        /// <summary>
        /// The current colors of all answer hints
        /// </summary>
        [JsonIgnore]
        public IList<string> HintTextColor { get; set; }

        /// <summary>
        /// List that contains the complete test pool (one of this test will be ask each test round)
        /// </summary>
        [JsonIgnore]
        public IEnumerable<TestBaseModel> TestPool { get; set; }

        /// <summary>
        /// List with all possible tests
        /// </summary>
        public IEnumerable<TestBaseModel> AllTestsList { get; set; }

        /// <summary>
        /// A list with possible answers
        /// </summary>
        [JsonIgnore]
        public IEnumerable<TestBaseModel> PossibleAnswers { get; set; }

        /// <summary>
        /// The previous tests
        /// </summary>
        [JsonIgnore]
        public TestBaseModel PreviousTest
        {
            get => _previousTest;
            set
            {
                if(_previousTest == value)
                {
                    return;
                }

                _previousTest = value;
                OnPropertyChanged(nameof(CanGoToLastTest));
            }
        }

        /// <summary>
        /// The current sign quest
        /// </summary>

        [JsonIgnore]
        public TestBaseModel CurrentTest { get; set; }

        /// <summary>
        /// The time stamp when the test was start
        /// </summary>
        [JsonIgnore]
        public DateTime TestStartTime
        {
            get => _testStartTime;
            set
            {
                if(_testStartTime == value)
                {
                    return;
                }

                _testStartTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoToLastTest));
            }
        }

        /// <summary>
        /// The current answer time of a test
        /// </summary>
        public TimeSpan AnswerTime
        {
            get => _answerTime;
            set
            {
                if(_answerTime == value)
                {
                    return;
                }

                _answerTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The timeout for highlight a wrong and/or a correct answered question in milliseconds
        /// </summary>
        public TimeSpan HighlightTimeout
        {
            get => _highlightTimeout;
            set
            {
                if(_highlightTimeout == value)
                {
                    return;
                }

                _highlightTimeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The maximum answer timeout in milliseconds
        /// </summary>
        public TimeSpan MaximumAnswerTimeout
        {
            get => _maximumAnswerTimeout;
            set
            {
                if(_maximumAnswerTimeout == value)
                {
                    return;
                }

                _maximumAnswerTimeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current ask sign
        /// </summary>
        [JsonIgnore]
        public string CurrentAskSign { get; set; }

        /// <summary>
        /// The current color of the ask sign
        /// </summary>
        [JsonIgnore]
        public string CurrentAskSignColor
        {
            get => _currentAskSignColor;
            set
            {
                if(_currentAskSignColor == value)
                {
                    return;
                }

                _currentAskSignColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The color for the progress bar (running answer time)
        /// </summary>
        [JsonIgnore]
        public string ProgressBarColor
        {
            get => _progressBarColor;
            set
            {
                if(_progressBarColor == value)
                {
                    return;
                }

                _progressBarColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Return the version and the target framework of this library
        /// </summary>
        [JsonIgnore]
        public string GetLibraryVersion
            => $"{AssemblyHelper.GetAssemblyVersion(this)} ({AssemblyHelper.GetTargetFramework(this)})";

        [JsonIgnore]
        public string WrongAnswerCountString
            => $"H: {AllTestsList.Sum(found => found.WrongHiraganaCount)}"
             + $" K: {AllTestsList.Sum(found => found.WrongKatakanaCount)}";

        [JsonIgnore]
        public string RightAnswerCountString
            => $"H: {AllTestsList.Sum(found => found.CorrectHiraganaCount)}"
             + $" K: {AllTestsList.Sum(found => found.CorrectKatakanaCount)}";

        /// <summary>
        /// Return a short <see cref="string"/> for the current <see cref="SelectedTestType"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [JsonIgnore]
        public string TestTypeString
            => SelectedTestType switch
            {
                TestType.HiraganaOrKatakanaToRoomaji            => "H / K => R",
                TestType.HiraganaToRoomaji                      => "H => R",
                TestType.KatakanaToRoomaji                      => "K => R",
                TestType.RoomajiToHiraganaOrKatakana            => "R => H / K",
                TestType.RoomajiToHiragana                      => "R => H",
                TestType.RoomajiToKatakana                      => "R => K",
                TestType.HiraganaToKatakanaOrKatakanaToHiragana => "H => K / K => H",
                TestType.HiraganaToKatakana                     => "H => K",
                TestType.KatakanaToHiragana                     => "K => H",
                TestType.AllToAll                               => "All => All",
                _                                               => throw new ArgumentOutOfRangeException(nameof(SelectedTestType), "Test type not supported"),
            };

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

        /// <summary>
        /// A readable <see cref="string"/> with count of wrong answers for the <see cref="CurrentTest"/>, based on the <see cref="SelectedTestType"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [JsonIgnore]
        public string WrongCount
        {
            get
            {
                if(CurrentTest is null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.WrongHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.WrongKatakanaCount}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Roomaji:
                        return $"H: {CurrentTest.CorrectHiraganaCount} - K: {CurrentTest.CorrectKatakanaCount}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        /// <summary>
        /// A readable <see cref="string"/> with count of correct answers for the <see cref="CurrentTest"/>, based on the <see cref="SelectedTestType"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [JsonIgnore]
        public string CorrectCount
        {
            get
            {
                if(CurrentTest is null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.CorrectHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.CorrectKatakanaCount}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Roomaji:
                        return $"H: {CurrentTest.CorrectHiraganaCount} - K: {CurrentTest.CorrectKatakanaCount}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        /// <summary>
        /// A readable <see cref="string"/> with the average answer time for the <see cref="CurrentTest"/>, based on the <see cref="SelectedTestType"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [JsonIgnore]
        public string AverageAnswerTime
        {
            get
            {
                if(CurrentTest is null)
                {
                    return string.Empty;
                }

                switch(SelectedTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Hiragana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Hiragana:
                        return $"{CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                    case TestType.HiraganaToKatakanaOrKatakanaToHiragana when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.HiraganaOrKatakanaToRoomaji when CurrentAskSign == CurrentTest.Katakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Katakana:
                        return $"{CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";

                    case TestType.RoomajiToHiraganaOrKatakana:
                    case TestType.AllToAll when CurrentAskSign == CurrentTest.Roomaji:
                        return $"H: {CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff} - K: {CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(SelectedTestType), SelectedTestType, "test type is not supported");
                }
            }
        }

        [JsonIgnore]
        public string CorrectCountIndicator
        {
            get => _correctCountIndicator;
            set
            {
                if(_correctCountIndicator == value)
                {
                    return;
                }

                _correctCountIndicator = value;
            }
        }

        [JsonIgnore]
        public string WrongCountIndicator
        {
            get => _wrongCountIndicator;
            set
            {
                if(_wrongCountIndicator == value)
                {
                    return;
                }

                _wrongCountIndicator = value;
            }
        }

        [JsonIgnore]
        public string AverageAnswerTimeIndicator
        {
            get => _averageAnswerTimeIndicator;
            set
            {
                if(_averageAnswerTimeIndicator == value)
                {
                    return;
                }

                _averageAnswerTimeIndicator = value;
            }
        }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double WindowHigh { get; set; }

        public double WindowWidth { get; set; }

        /// <summary>
        /// The current selected test type
        /// </summary>
        public TestType SelectedTestType
        {
            get => _selectedTestType;
            set
            {
                if(_selectedTestType == value)
                {
                    return;
                }

                _selectedTestType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TestTypeString));
            }
        }

        /// <summary>
        /// The current selected hint type
        /// </summary>
        public HintType SelectedHintType
        {
            get => _selectedHintType;
            set
            {
                if(_selectedHintType == value)
                {
                    return;
                }

                _selectedHintType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The kana types to test (flag enum value)
        /// </summary>
        public KanaType SelectedKanaType
        {
            get => _selectedKanaType;
            set
            {
                if(_selectedKanaType == value)
                {
                    return;
                }

                _selectedKanaType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The type of hints that should show, when a answer was wrong and/or correct
        /// </summary>
        public HintShowType SelectedHintShowType
        {
            get => _selectedHintShowType;
            set
            {
                if(_selectedHintShowType == value)
                {
                    return;
                }

                _selectedHintShowType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of maximum answers
        /// </summary>
        public byte MaximumAnswers
        {
            get => _maximumAnswers;
            set
            {
                if(_maximumAnswers == value)
                {
                    return;
                }

                _maximumAnswers = value;
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
                if(_showAnswerShortcuts == value)
                {
                    return;
                }

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
                if(_showRunningAnswerTimer == value)
                {
                    return;
                }

                _showRunningAnswerTimer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the signs statistics will be shown
        /// </summary>
        public bool ShowStatistics
        {
            get => _showStatistics;
            set
            {
                if(_showStatistics == value)
                {
                    return;
                }

                _showStatistics = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the program check on every start-up for a new version
        /// </summary>
        public bool CheckForNewVersionOnStartUp
        {
            get => _checkForNewVersionOnStartUp;
            set
            {
                if(_checkForNewVersionOnStartUp == value)
                {
                    return;
                }

                _checkForNewVersionOnStartUp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that answers will be highlighted on a wrong answer
        /// </summary>
        public bool HighlightOnWrongAnswer
        {
            get => _highlightOnWrongAnswer;
            set
            {
                if(_highlightOnWrongAnswer == value)
                {
                    return;
                }

                _highlightOnWrongAnswer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the answer timer is used
        /// </summary>
        public bool UseAnswerTimer
        {
            get => _useAnswerTimer;
            set
            {
                if(_useAnswerTimer == value)
                {
                    return;
                }

                _useAnswerTimer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that answers will be highlighted on a correct answer
        /// </summary>
        public bool HighlightOnCorrectAnswer
        {
            get => _highlightOnCorrectAnswer;
            set
            {
                if(_highlightOnCorrectAnswer == value)
                {
                    return;
                }

                _highlightOnCorrectAnswer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the user can go to the previous test
        /// </summary>
        [JsonIgnore]
        public bool CanGoToLastTest
            => !PreviousTest.Equals(TestBaseModel.EmptyTest);

        /// <summary>
        /// Indicate that the current input (mouse, keyboard, game-pad and menu) will ignore and no processed
        /// </summary>
        [JsonIgnore]
        public bool IgnoreInput { get; set; }

        /// <summary>
        /// Indicate that only similar answers will be shown as possible answers
        /// </summary>
        public bool SimilarAnswers
        {
            get => _similarAnswers;
            set
            {
                if(_similarAnswers == value)
                {
                    return;
                }

                _similarAnswers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the correct counter is highlighted
        /// </summary>
        [JsonIgnore]
        public bool HighlightCorrectCounter
        {
            get => _highlightCorrectCounter;
            set
            {
                if(_highlightCorrectCounter == value)
                {
                    return;
                }

                _highlightCorrectCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the wrong counter is highlighted
        /// </summary>
        [JsonIgnore]
        public bool HighlightWrongCounter
        {
            get => _highlightWrongCounter;
            set
            {
                if(_highlightWrongCounter == value)
                {
                    return;
                }

                _highlightWrongCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the answer time is highlighted
        /// </summary>
        [JsonIgnore]
        public bool HighlightAnswerTime
        {
            get => _highlightAnswerTime;
            set
            {
                if(_highlightAnswerTime == value)
                {
                    return;
                }

                _highlightAnswerTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that the statistics of the current ask sign is shown
        /// </summary>
        public bool ShowSignStatistics
        {
            get => _showSignStatistics;
            set
            {
                if(_showSignStatistics == value)
                {
                    return;
                }

                _showSignStatistics = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicate that all answers have only the same kana type (make tests a little bit harder)
        /// </summary>
        public bool ShowOnlySameKanaOnAnswers
        {
            get => _showOnlySameKanaOnAnswers;
            set
            {
                if(_showOnlySameKanaOnAnswers == value)
                {
                    return;
                }

                _showOnlySameKanaOnAnswers = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Global random generator
        /// </summary>
        internal Random Randomizer { get; set; }

        #endregion Internal Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="PreviousTest"/>
        /// </summary>
        private TestBaseModel _previousTest;

        /// <summary>
        /// Backing-filed for <see cref="TestStartTime"/>
        /// </summary>
        private DateTime _testStartTime;

        /// <summary>
        /// Backing-field for <see cref="AnswerTime"/>
        /// </summary>
        private TimeSpan _answerTime;

        /// <summary>
        /// Backing-field for <see cref="MaximumAnswerTimeout"/>
        /// </summary>
        private TimeSpan _maximumAnswerTimeout;

        /// <summary>
        /// Backing-field for <see cref="HighlightTimeout"/>
        /// </summary>
        private TimeSpan _highlightTimeout;

        /// <summary>
        /// Backing-field for <see cref="CurrentAskSignColor"/>
        /// </summary>
        private string _currentAskSignColor;

        /// <summary>
        /// Backing-field for <see cref="ProgressBarColor"/>
        /// </summary>
        private string _progressBarColor;

        /// <summary>
        /// Backing-field for <see cref="AverageAnswerTimeIndicator"/>
        /// </summary>
        private string _averageAnswerTimeIndicator;

        /// <summary>
        /// Backing-field for <see cref="CorrectCountIndicator"/>
        /// </summary>
        private string _correctCountIndicator;

        /// <summary>
        /// Backing-field for <see cref="WrongCountIndicator"/>
        /// </summary>
        private string _wrongCountIndicator;

        /// <summary>
        /// Backing-field for <see cref="SelectedHintShowType"/>
        /// </summary>
        private HintShowType _selectedHintShowType;

        /// <summary>
        /// Backing-field for <see cref="SelectedHintType"/>
        /// </summary>
        private HintType _selectedHintType;

        /// <summary>
        /// Backing-field for <see cref="SelectedKanaType"/>
        /// </summary>
        private KanaType _selectedKanaType;

        /// <summary>
        /// Backing-field for <see cref="SelectedTestType"/>
        /// </summary>
        private TestType _selectedTestType;

        /// <summary>
        /// Backing-field for <see cref="MaximumAnswers"/>
        /// </summary>
        private byte _maximumAnswers;

        /// <summary>
        /// Backing-field for <see cref="SimilarAnswers"/>
        /// </summary>
        private bool _similarAnswers;

        /// <summary>
        /// Backing-filed for <see cref="ShowAnswerShortcuts"/>
        /// </summary>
        private bool _showAnswerShortcuts;

        /// <summary>
        /// Backing-field for <see cref="ShowRunningAnswerTimer"/>
        /// </summary>
        private bool _showRunningAnswerTimer;

        /// <summary>
        /// Backing-field for <see cref="ShowStatistics"/>
        /// </summary>
        private bool _showStatistics;

        /// <summary>
        /// Backing-field for <see cref="CheckForNewVersionOnStartUp"/>
        /// </summary>
        private bool _checkForNewVersionOnStartUp;

        /// <summary>
        /// Backing-field for <see cref="HighlightOnWrongAnswer"/>
        /// </summary>
        private bool _highlightOnWrongAnswer;

        /// <summary>
        /// Backing-field for <see cref="HighlightOnCorrectAnswer"/>
        /// </summary>
        private bool _highlightOnCorrectAnswer;

        /// <summary>
        /// Backing-field for <see cref="UseAnswerTimer"/>
        /// </summary>
        private bool _useAnswerTimer;

        /// <summary>
        /// Backing-field for <see cref="HighlightCorrectCounter"/>
        /// </summary>
        private bool _highlightCorrectCounter;

        /// <summary>
        /// Backing-field for <see cref="HighlightWrongCounter"/>
        /// </summary>
        private bool _highlightWrongCounter;

        /// <summary>
        /// Backing-field for <see cref="HighlightAnswerTime"/>
        /// </summary>
        private bool _highlightAnswerTime;

        /// <summary>
        /// Backing-field for <see cref="ShowSignStatistics"/>
        /// </summary>
        private bool _showSignStatistics;

        /// <summary>
        /// Backing-field for <see cref="ShowOnlySameKanaOnAnswers"/>
        /// </summary>
        private bool _showOnlySameKanaOnAnswers;

        #endregion Private Backing-Fields

        #region Public Constructors

        /// <summary>
        /// Create a new <see cref="MainBaseModel"/> with default values
        /// </summary>
        public MainBaseModel()
        {
            HighlightTimer               = new ManualResetEvent(false);
            Randomizer                   = new Random();

            CurrentTest                  = TestBaseModel.EmptyTest;
            _previousTest                = TestBaseModel.EmptyTest;

            AnswerButtonColor            = new List<string>(10);
            HintTextColor                = new List<string>(10);

            PossibleAnswers              = new Collection<TestBaseModel>();
            TestPool                     = new Collection<TestBaseModel>();
            AllTestsList                 = new Collection<TestBaseModel>();

            CurrentAskSign               = string.Empty;
            _averageAnswerTimeIndicator  = string.Empty;
            _correctCountIndicator       = string.Empty;
            _wrongCountIndicator         = string.Empty;
            _progressBarColor            = "#FFADD8E6"; // Colors.LightBlue
            _currentAskSignColor         = "#00FFFFFF"; // Colors.Transparent

            LeftPosition                 = double.NaN;
            TopPosition                  = double.NaN;
            WindowHigh                   = double.NaN;
            WindowWidth                  = double.NaN;

            _maximumAnswerTimeout        = new TimeSpan(0, 0, 10);
            _highlightTimeout            = new TimeSpan(0, 0, 3);

            _maximumAnswers              = 7;

            _selectedTestType            = TestType.HiraganaOrKatakanaToRoomaji;
            _selectedHintType            = HintType.BasedOnAskSign;

            _selectedHintShowType        = HintShowType.ShowOnCorrectAnswer
                                         | HintShowType.ShowOnWrongAnswer
                                         | HintShowType.ShowOnMarkedAnswers
                                         | HintShowType.ShowOnOtherAnswers;

            _selectedKanaType            = KanaType.Gojuuon
                                         | KanaType.GojuuonWithDakuten
                                         | KanaType.GojuuonWithHandakuten
                                         | KanaType.Yooon
                                         | KanaType.YooonWithDakuten
                                         | KanaType.YooonWithHandakuten;

            _showStatistics              = false;
            _showAnswerShortcuts         = true;
            _showRunningAnswerTimer      = true;
            _similarAnswers              = true;
            _checkForNewVersionOnStartUp = true;
            _highlightOnWrongAnswer      = true;
            _useAnswerTimer              = true;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Refresh the most surface properties
        /// (Call <see cref="PropertyChangedHelper.OnPropertyChanged(in string)"/> for the most properties)
        /// </summary>
        public void OnPropertyChangeForAll()
        {
            OnPropertyChanged(nameof(CurrentAskSign));
            OnPropertyChanged(nameof(AllTestsList));

            OnPropertyChangedOnlyForStatistics();
        }

        /// <summary>
        /// Refresh all statistic properties
        /// (Call <see cref="PropertyChangedHelper.OnPropertyChanged(in string)"/> for all statistic properties)
        /// </summary>
        public void OnPropertyChangedOnlyForStatistics()
        {
            OnPropertyChanged(nameof(WrongCount));
            OnPropertyChanged(nameof(CorrectCount));
            OnPropertyChanged(nameof(AverageAnswerTime));

            OnPropertyChanged(nameof(CorrectCountIndicator));
            OnPropertyChanged(nameof(WrongCountIndicator));
            OnPropertyChanged(nameof(AverageAnswerTimeIndicator));

            OnPropertyChanged(nameof(WrongAnswerCountString));
            OnPropertyChanged(nameof(RightAnswerCountString));
            OnPropertyChanged(nameof(CurrentRateText));
            OnPropertyChanged(nameof(TestPool));
        }

        public void OnPropertyChangedForAnswerButtonColors()
        {
            OnPropertyChanged(nameof(AnswerButtonColor));
            OnPropertyChanged(nameof(HintTextColor));
        }

        /// <summary>
        /// Check if all values of this model are in range and use default values, when not
        /// </summary>
        public void CheckAndFixValues()
            {
            if(double.IsInfinity(LeftPosition))
            {
                LeftPosition = double.NaN;
            }

            if(double.IsInfinity(TopPosition))
            {
                TopPosition = double.NaN;
            }

            if(double.IsInfinity(WindowHigh))
            {
                WindowHigh = double.NaN;
            }

            if(double.IsInfinity(WindowWidth))
            {
                WindowWidth = double.NaN;
            }

            var minTime = new TimeSpan(0, 0, 1);
            var maxTime = new TimeSpan(0, 5, 0);

            if(HighlightTimeout < minTime || HighlightTimeout > maxTime)
            {
                Debug.WriteLine($"[{nameof(HighlightTimeout)}] is not between {minTime.ToString()} and {maxTime.ToString()}");
                HighlightTimeout = maxTime;
            }

            if(MaximumAnswerTimeout >= minTime && MaximumAnswerTimeout <= maxTime)
            {
                return;
            }

            Debug.WriteLine($"[{nameof(MaximumAnswerTimeout)}] is not between {minTime.ToString()} and {maxTime.ToString()}");
            MaximumAnswerTimeout = maxTime;
        }

        #endregion Public Methods

        #region IDisposable Implementation

        public void Dispose()
        {
            HighlightTimer.Dispose();

            PreviousTest         = TestBaseModel.EmptyTest;
            CurrentTest          = TestBaseModel.EmptyTest;

            AnswerButtonColor.Clear();
            HintTextColor.Clear();

            TestPool             = Enumerable.Empty<TestBaseModel>();
            AllTestsList         = Enumerable.Empty<TestBaseModel>();
            PossibleAnswers      = Enumerable.Empty<TestBaseModel>();

            CurrentAskSign       = string.Empty;
            CurrentAskSignColor  = string.Empty;
            ProgressBarColor     = string.Empty;

            TestStartTime        = DateTime.MinValue;
            AnswerTime           = TimeSpan.Zero;
            HighlightTimeout     = TimeSpan.Zero;
            MaximumAnswerTimeout = TimeSpan.Zero;

            LeftPosition         = double.NaN;
            TopPosition          = double.NaN;
            WindowHigh           = double.NaN;
            WindowWidth          = double.NaN;
        }

        #endregion IDisposable Implementation
    }
}
