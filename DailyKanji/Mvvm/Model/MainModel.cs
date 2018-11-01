using DailyKanji.Enumerations;
using DailyKanji.Helper;
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
        public TestModel CurrentTest
        {
            get => _currentTest;
            internal set
            {
                _currentTest = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with possible answers
        /// </summary>
        public ObservableCollection<TestBaseModel> PossibleAnswers
        {
            get => _possibleAnswers;
            internal set
            {
                _possibleAnswers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of right answers
        /// </summary>
        public uint RightAnswerCount
        {
            get => _rightAnswerCount;
            internal set
            {
                _rightAnswerCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentRateText));
            }
        }

        /// <summary>
        /// The current colour of all answer buttons
        /// </summary>
        public ObservableCollection<Brush> AnswerButtonColor
        {
            get => _buttonColor;
            internal set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current color of the ask sign
        /// </summary>
        public Brush CurrentAskSignColor
        {
            get => _currentAskSignColor;
            internal set
            {
                _currentAskSignColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current ask sign
        /// </summary>
        public string CurrentAskSign
        {
            get => _currentAskSign;
            internal set
            {
                _currentAskSign = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WrongAnswers));
            }
        }

        /// <summary>
        /// Return the current rate in percent
        /// </summary>
        public string CurrentRateText
        {
            get
            {
                var wrongAnswerCount = AllTestsList.Sum(found => found.WrongHiragana + found.WrongKatakana);

                return wrongAnswerCount != 0
                    ? $"{Math.Round(100.0 / (wrongAnswerCount + RightAnswerCount) * RightAnswerCount, 2)}%"
                    : "100%";
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

        public IReadOnlyCollection<TestModel> NewQuestionList
        {
            get => _newQuestionList;
            internal set
            {
                _newQuestionList = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuestionPoolString));
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
            }
        }

        public string QuestionPoolString
            => $"H: {NewQuestionList.Count(found => found.TestType == TestType.HiraganaToRoomaji)}"
             + $" K: {NewQuestionList.Count(found => found.TestType == TestType.KatakanaToRoomaji)}";

        public IEnumerable<TestBaseModel> WrongAnswers
            => AllTestsList.Where(found => found.WrongHiragana > 0 || found.WrongKatakana > 0);

        public string TestTypeString
        {
            get
            {
                switch(MainTestType)
                {
                    case TestType.HiraganaToRoomaji:
                        return "H => R";

                    case TestType.KatakanaToRoomaji:
                        return "K => R";

                    default:
                        return "H / K => R";
                }
            }
        }

        /// <summary>
        /// Global random generator
        /// </summary>
        internal Random Randomizer { get; }

        /// <summary>
        /// List with all possible tests
        /// </summary>
        public IReadOnlyCollection<TestBaseModel> AllTestsList { get; }

        /// <summary>
        /// Indicate that the current input (mouse and keyboard) will ignore and no processed
        /// </summary>
        internal bool IgnoreInput { get; set; }

        #endregion Public Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="CurrentTest"/>
        /// </summary>
        private TestModel _currentTest;

        /// <summary>
        /// Backing-field for <see cref="PossibleAnswers"/>
        /// </summary>
        private ObservableCollection<TestBaseModel> _possibleAnswers;

        /// <summary>
        /// Backing-field for <see cref="RightAnswerCount"/>
        /// </summary>
        private uint _rightAnswerCount;

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
        private IReadOnlyCollection<TestModel> _newQuestionList;

        /// <summary>
        /// Backing-field for <see cref="ErrorTimeout"/>
        /// </summary>
        private int _errorTimeout;
        private TestType _mainTestType;

        #endregion Private Backing-Fields

        #region Public Constructors

        public MainModel()
        {
            MainTestType      = TestType.HiraganaOrKatakanaToRomaji;
            ErrorTimeout      = 1_500;
            MaximumAnswer     = 5;
            Randomizer        = new Random();
            AnswerButtonColor = new ObservableCollection<Brush>();
            PossibleAnswers   = new ObservableCollection<TestBaseModel>();
            NewQuestionList   = new Collection<TestModel>();

            AllTestsList = new Collection<TestBaseModel>
            {
                // Kana Signs, Think Now How Much You Really Want (to learn them).
                new TestBaseModel("a", "あ", "ア"),
                new TestBaseModel("i", "い", "イ"),
                new TestBaseModel("u", "う", "ウ"),
                new TestBaseModel("e", "え", "エ"),
                new TestBaseModel("o", "お", "オ"),

                new TestBaseModel("ka", "か", "カ"),
                new TestBaseModel("ki", "き", "キ"),
                new TestBaseModel("ku", "く", "ク"),
                new TestBaseModel("ke", "け", "ケ"),
                new TestBaseModel("ko", "こ", "コ"),

                new TestBaseModel("sa", "さ", "サ"),
                new TestBaseModel("shi","し", "シ"),
                new TestBaseModel("su", "す", "ス"),
                new TestBaseModel("se", "せ", "セ"),
                new TestBaseModel("so", "そ", "ソ"),

                new TestBaseModel("ta", "た", "タ"),
                new TestBaseModel("chi","ち", "チ"),
                new TestBaseModel("tsu","つ", "ツ"),
                new TestBaseModel("te", "て", "テ"),
                new TestBaseModel("to", "と", "ト"),

                new TestBaseModel("na", "な", "ナ"),
                new TestBaseModel("ni", "に", "ニ"),
                new TestBaseModel("nu", "ぬ", "ヌ"),
                new TestBaseModel("ne", "ね", "ネ"),
                new TestBaseModel("no", "の", "ノ"),

                new TestBaseModel("ha", "は", "ハ"),
                new TestBaseModel("hi", "ひ", "ヒ"),
                new TestBaseModel("fu", "ふ", "フ"),
                new TestBaseModel("he", "へ", "ヘ"),
                new TestBaseModel("ho", "ほ", "ホ"),

                new TestBaseModel("ma", "ま", "マ"),
                new TestBaseModel("mi", "み", "ミ"),
                new TestBaseModel("mu", "む", "ム"),
                new TestBaseModel("me", "め", "メ"),
                new TestBaseModel("mo", "も", "モ"),

                new TestBaseModel("ya", "や", "ヤ"),
                new TestBaseModel("yu", "ゆ", "ユ"),
                new TestBaseModel("yo", "よ", "ヨ"),

                new TestBaseModel("ra", "ら", "ラ"),
                new TestBaseModel("ri", "り", "リ"),
                new TestBaseModel("ru", "る", "ル"),
                new TestBaseModel("re", "れ", "レ"),
                new TestBaseModel("ro", "ろ", "ロ"),

                new TestBaseModel("wa", "わ", "ワ"),
                new TestBaseModel("wo", "を", "ヲ"),

                new TestBaseModel("n",  "ん", "ン")
            };
        }

        #endregion Public Constructors
    }
}
