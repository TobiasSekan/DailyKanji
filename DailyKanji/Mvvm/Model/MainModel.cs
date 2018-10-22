using DailyKanji.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper
    {
        #region Public Properties

        /// <summary>
        /// Indicate that the current input (mouse and keyboard) will ignore and no processed
        /// </summary>
        public bool IgnoreInput { get; set; }

        public int MainWindowWidth
        {
            get => _mainWindowWidth;
            set
            {
                _mainWindowWidth = value;
                OnPropertyChanged();
            }
        }

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
        public ObservableCollection<TestModel> PossibleAnswers
        {
            get => _possibleAnswers;
            internal set
            {
                _possibleAnswers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with all wrong answered tests
        /// </summary>
        public ObservableCollection<TestModel> WrongAnswers
        {
            get => _wrongAnswers;
            internal set
            {
                _wrongAnswers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of right answers
        /// </summary>
        public uint RightAnswerCount
        {
            get => _rightAnswerCount;
            set
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
            set
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
            set
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
            set
            {
                _currentAskSign = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Return the current rate in percent
        /// </summary>
        public string CurrentRateText
            => RightAnswerCount != 0 || WrongAnswers.Count != 0
                ? $"{Math.Round(100.0 / (WrongAnswers.Count + RightAnswerCount) * RightAnswerCount, 2)}%"
                : "100%";

        /// <summary>
        /// List with all possible tests
        /// </summary>
        public IReadOnlyCollection<TestModel> TestList { get; }

        /// <summary>
        /// Global random generator
        /// </summary>
        public Random Randomizer { get; }

        /// <summary>
        /// The count of maximum answers
        /// </summary>
        public byte MaximumAnswer { get; set; }

        #endregion Public Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="CurrentTest"/>
        /// </summary>
        private TestModel _currentTest;

        /// <summary>
        /// Backing-field for <see cref="PossibleAnswers"/>
        /// </summary>
        private ObservableCollection<TestModel> _possibleAnswers;

        /// <summary>
        /// Backing-field for <see cref="WrongAnswers"/>
        /// </summary>
        private ObservableCollection<TestModel> _wrongAnswers;

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
        /// Backing-field for <see cref="MainWindowWidth"/>
        /// </summary>
        private int _mainWindowWidth;

        #endregion Private Backing-Fields

        #region Public Constructors

        public MainModel()
        {
            MaximumAnswer     = 5;
            MainWindowWidth   = 600;
            Randomizer        = new Random();
            AnswerButtonColor = new ObservableCollection<Brush>();
            PossibleAnswers   = new ObservableCollection<TestModel>();
            WrongAnswers      = new ObservableCollection<TestModel>();

            TestList = new Collection<TestModel>
            {
                // Kana Signs, Think Now How Much You Really Want (to learn them).
                new TestModel("a", "あ", "ア"),
                new TestModel("i", "い", "イ"),
                new TestModel("u", "う", "ウ"),
                new TestModel("e", "え", "エ"),
                new TestModel("o", "お", "オ"),

                new TestModel("ka", "か", "カ"),
                new TestModel("ki", "き", "キ"),
                new TestModel("ku", "く", "ク"),
                new TestModel("ke", "け", "ケ"),
                new TestModel("ko", "こ", "コ"),

                new TestModel("sa", "さ", "サ"),
                new TestModel("shi","し", "シ"),
                new TestModel("su", "す", "ス"),
                new TestModel("se", "せ", "セ"),
                new TestModel("so", "そ", "ソ"),

                new TestModel("ta", "た", "タ"),
                new TestModel("chi","ち", "チ"),
                new TestModel("tsu","つ", "ツ"),
                new TestModel("te", "て", "テ"),
                new TestModel("to", "と", "ト"),

                new TestModel("na", "な", "ナ"),
                new TestModel("ni", "に", "ニ"),
                new TestModel("nu", "ぬ", "ヌ"),
                new TestModel("ne", "ね", "ネ"),
                new TestModel("no", "の", "ノ"),

                new TestModel("ha", "は", "ハ"),
                new TestModel("hi", "ひ", "ヒ"),
                new TestModel("fu", "ふ", "フ"),
                new TestModel("he", "へ", "ヘ"),
                new TestModel("ho", "ほ", "ホ"),

                new TestModel("ma", "ま", "マ"),
                new TestModel("mi", "み", "ミ"),
                new TestModel("mu", "む", "ム"),
                new TestModel("me", "め", "メ"),
                new TestModel("mo", "も", "モ"),

                new TestModel("ya", "や", "ヤ"),
                new TestModel("yu", "ゆ", "ユ"),
                new TestModel("yo", "よ", "ヨ"),

                new TestModel("ra", "ら", "ラ"),
                new TestModel("ri", "り", "リ"),
                new TestModel("ru", "る", "ル"),
                new TestModel("re", "れ", "レ"),
                new TestModel("ro", "ろ", "ロ"),

                new TestModel("wa", "わ", "ワ"),
                new TestModel("wo", "を", "ヲ"),

                new TestModel("n",  "ん", "ン")
            };
        }

        #endregion Public Constructors
    }
}
