using DailyKanji.Enumerations;
using DailyKanji.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                OnPropertyChanged(nameof(WrongCount));
                OnPropertyChanged(nameof(CorrectCount));
                OnPropertyChanged(nameof(AverageAnswerTime));
            }
        }

        /// <summary>
        /// A list with possible answers
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<TestBaseModel> PossibleAnswers
        {
            get => _possibleAnswers;
            set
            {
                _possibleAnswers = value;
                OnPropertyChanged();
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

        public bool SimilarAnswers
        {
            get => _similarAnswers;
            set
            {
                _similarAnswers = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public IReadOnlyCollection<TestBaseModel> NewQuestionList
        {
            get => _newQuestionList;
            set
            {
                _newQuestionList = value;
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

        public TestType MainTestType
        {
            get => _mainTestType;
            set
            {
                _mainTestType = value;
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
                switch(MainTestType)
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

                switch(MainTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                        return $"{CurrentTest.WrongHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        return $"{CurrentTest.WrongKatakanaCount}";

                    case TestType.HiraganaOrKatakanaToRoomaji:
                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.WrongHiraganaCount} - K: {CurrentTest.WrongKatakanaCount}";
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

                switch(MainTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                        return $"{CurrentTest.CorrectHiraganaCount}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        return $"{CurrentTest.CorrectKatakanaCount}";

                    case TestType.HiraganaOrKatakanaToRoomaji:
                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.CorrectHiraganaCount} - K: {CurrentTest.CorrectKatakanaCount}";
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

                switch(MainTestType)
                {
                    case TestType.HiraganaToRoomaji:
                    case TestType.HiraganaToKatakana:
                    case TestType.RoomajiToHiragana:
                        return $"{CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff}";

                    case TestType.KatakanaToRoomaji:
                    case TestType.RoomajiToKatakana:
                    case TestType.KatakanaToHiragana:
                        return $"{CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";

                    case TestType.HiraganaOrKatakanaToRoomaji:
                    case TestType.RoomajiToHiraganaOrKatakana:
                        return $"H: {CurrentTest.AverageAnswerTimeForHiragana:mm\\:ss\\.ff} - K: {CurrentTest.AverageAnswerTimeForKatakana:mm\\:ss\\.ff}";
                }

                return string.Empty;
            }
        }

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
        internal DateTime TestStartTime { get; set; }

        #endregion Public Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="CurrentTest"/>
        /// </summary>
        private TestBaseModel _currentTest;

        /// <summary>
        /// Backing-field for <see cref="PossibleAnswers"/>
        /// </summary>
        private ObservableCollection<TestBaseModel> _possibleAnswers;

        /// <summary>
        /// Backing-field for <see cref="AnswerButtonColor"/>
        /// </summary>
        private ObservableCollection<Brush> _buttonColor;

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
        /// Backing-field for <see cref="NewQuestionList"/>
        /// </summary>
        private IReadOnlyCollection<TestBaseModel> _newQuestionList;

        /// <summary>
        /// Backing-field for <see cref="ErrorTimeout"/>
        /// </summary>
        private int _errorTimeout;

        /// <summary>
        /// Backing-field for <see cref="MainTestType"/>
        /// </summary>
        private TestType _mainTestType;

        /// <summary>
        /// Backing-field for <see cref="AllTestsList"/>
        /// </summary>
        private ICollection<TestBaseModel> _allTestsList;

        #endregion Private Backing-Fields

        #region Public Constructors

        public MainModel()
        {
            MainTestType   = TestType.HiraganaOrKatakanaToRoomaji;
            ErrorTimeout   = 1_500;
            MaximumAnswer  = 5;
            SimilarAnswers = true;
        }

        #endregion Public Constructors
    }
}
