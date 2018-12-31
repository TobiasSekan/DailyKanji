using DailyKanjiLogic.Helper;
using Newtonsoft.Json;
using System;

namespace DailyKanjiLogic.Mvvm.Model
{
    public sealed class TestBaseModel : PropertyChangedHelper, IEquatable<TestBaseModel>, IFormattable
    {
        #region Public Properties

        /// <summary>
        /// The sign in Roomaji
        /// </summary>
        public string Roomaji { get; set; }

        /// <summary>
        /// The sign in Hiragana
        /// </summary>
        public string Hiragana { get; set; }

        /// <summary>
        /// The sign in Katakana
        /// </summary>
        public string Katakana { get; set; }

        /// <summary>
        /// The count of wrong answers when the Hiragana sign was ask
        /// </summary>
        public uint WrongHiraganaCount
        {
            get => _wrongHiraganaCount;
            set
            {
                _wrongHiraganaCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of wrong answers when the Katakana sign was ask
        /// </summary>
        public uint WrongKatakanaCount
        {
            get => _wrongKatakanaCount;
            set
            {
                _wrongKatakanaCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of correct answers when the Hiragana sign was ask
        /// </summary>
        public uint CorrectHiraganaCount
        {
            get => _correctHiraganaCount;
            set
            {
                _correctHiraganaCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The count of correct answers when the Katakana sign was ask
        /// </summary>
        public uint CorrectKatakanaCount
        {
            get => _correctKatakanaCount;
            set
            {
                _correctKatakanaCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The long-term fail counter (+1 for each wrong ask question, -1 for each correct question)
        /// </summary>
        public uint WrongnessCounter
        {
            get => _wrongnessCounter;
            set
            {
                _wrongnessCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The complete answer time of all correct answered test with a Hiragana signs
        /// </summary>
        public TimeSpan CompleteAnswerTimeForCorrectHiragana
        {
            get => _completeAnswerTimeForCorrectHiragana;
            set
            {
                _completeAnswerTimeForCorrectHiragana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForHiragana));
            }
        }

        /// <summary>
        /// The complete answer time of all wrong answered test with a Hiragana signs
        /// </summary>
        public TimeSpan CompleteAnswerTimeForWrongHiragana
        {
            get => _completeAnswerTimeForWrongHiragana;
            set
            {
                _completeAnswerTimeForWrongHiragana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForHiragana));
            }
        }

        /// <summary>
        /// The complete answer time of all correct answered test with a Katakana sign
        /// </summary>
        public TimeSpan CompleteAnswerTimeForCorrectKatakana
        {
            get => _completeAnswerTimeForCorrectKatakana;
            set
            {
                _completeAnswerTimeForCorrectKatakana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForKatakana));
            }
        }

        /// <summary>
        /// The complete answer time of all wrong answered test with the a Katakana sign
        /// </summary>
        public TimeSpan CompleteAnswerTimeForWrongKatakana
        {
            get => _completeAnswerTimeForWrongKatakana;
            set
            {
                _completeAnswerTimeForWrongKatakana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForKatakana));
            }
        }

        /// <summary>
        /// The average answer time of all test with a Hiragana sign
        /// </summary>
        [JsonIgnore]
        public TimeSpan AverageAnswerTimeForHiragana
            => CorrectHiraganaCount + WrongHiraganaCount > 0
                ? new TimeSpan((CompleteAnswerTimeForCorrectHiragana.Ticks + CompleteAnswerTimeForWrongHiragana.Ticks)
                               / (CorrectHiraganaCount + WrongHiraganaCount))
                : new TimeSpan();

        /// <summary>
        /// The average answer time of all test with a Katakana sign
        /// </summary>
        [JsonIgnore]
        public TimeSpan AverageAnswerTimeForKatakana
            => CorrectKatakanaCount + WrongKatakanaCount > 0
                ? new TimeSpan((CompleteAnswerTimeForCorrectKatakana.Ticks + CompleteAnswerTimeForWrongKatakana.Ticks)
                               / (CorrectKatakanaCount + WrongKatakanaCount))
                : new TimeSpan();

        #endregion Public Properties

        #region Private Backing-fields

        /// <summary>
        /// Backing-field for <see cref="WrongHiraganaCount"/>
        /// </summary>
        private uint _wrongHiraganaCount;

        /// <summary>
        /// Backing-field for <see cref="WrongKatakanaCount"/>
        /// </summary>
        private uint _wrongKatakanaCount;

        /// <summary>
        /// Backing-field for <see cref="CorrectHiraganaCount"/>
        /// </summary>
        private uint _correctHiraganaCount;

        /// <summary>
        /// Backing-field for <see cref="CorrectKatakanaCount"/>
        /// </summary>
        private uint _correctKatakanaCount;

        /// <summary>
        /// Backing-field for <see cref="WrongnessCounter"/>
        /// </summary>
        private uint _wrongnessCounter;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForCorrectHiragana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForCorrectHiragana;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForWrongHiragana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForWrongHiragana;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForCorrectKatakana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForCorrectKatakana;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForWrongKatakana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForWrongKatakana;

        #endregion Private Backing-fields

        #region Constructors

        /// <summary>
        /// Create a new test, based on the given values
        /// </summary>
        /// <param name="roomaji">The sign in Roomaji</param>
        /// <param name="hiragana">The sign in Hiragana</param>
        /// <param name="katakana">The sign in Katakana</param>
        public TestBaseModel(string roomaji, string hiragana, string katakana)
        {
            // use of 'in' modifier will crash JSON serialisation

            Roomaji  = roomaji;
            Hiragana = hiragana;
            Katakana = katakana;
        }

        #endregion Constructors

        #region Class Overrides

        /// <summary>
        /// Compare this <see cref="TestBaseModel"/> with the given <see cref="object"/>
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare</param>
        /// <returns><c>true</c> when <see cref="object"/> is from type <see cref="TestBaseModel"/>
        /// and Roomaji, Hiragana and Katakana are the same, otherwise <c>false</c></returns>
        public override bool Equals(object obj)
            => Equals(obj as TestBaseModel);

        /// <summary>
        /// Return the hash code for this <see cref="TestBaseModel"/>
        /// </summary>
        /// <returns>The hash code for this <see cref="TestBaseModel"/></returns>
        public override int GetHashCode()
            => (Roomaji.GetHashCode() << 5) ^ (Hiragana.GetHashCode() << 5) ^ (Katakana.GetHashCode() << 5);

        /// <summary>
        /// Return a readable <see cref="string"/> of this <see cref="TestBaseModel"/>
        /// </summary>
        /// <returns>A readable <see cref="string"/></returns>
        public override string ToString()
            => $"Roomaji: {Roomaji}, Hiragana: {Hiragana}, Katakana: {Katakana}";

        #endregion Class Overrides

        #region IEquatable<TestBaseModel> Implementation

        /// <summary>
        /// Compare this <see cref="TestBaseModel"/> with the given <see cref="TestBaseModel"/>
        /// </summary>
        /// <param name="other">The <see cref="TestBaseModel"/> to compare</param>
        /// <returns><c>true</c> when Roomaji, Hiragana and Katakana are the same, otherwise <c>false</c></returns>
        public bool Equals(TestBaseModel other)
            => other != null
            && other.Roomaji == Roomaji
            && other.Hiragana == Hiragana
            && other.Katakana == Katakana;

        #endregion IEquatable<TestBaseModel> Implementation

        #region IFormattable Implementation

        /// <summary>
        /// Return a readable <see cref="string"/> for the given <paramref name="format"/>
        /// </summary>
        /// <param name="format">The format of the readable string
        /// (R, H, K, RO, HI, KA, Roomaji, Hiragana or Katakana)</param>
        /// <param name="formatProvider">not used and ignored</param>
        /// <returns>A readable <see cref="string"/></returns>
        /// <exception cref="FormatException"></exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch(format?.ToUpper())
            {
                case "R":
                case "RO":
                case "ROOMAJI":
                    return Roomaji;

                case "H":
                case "HI":
                case "HIRAGANA":
                    return Hiragana;

                case "K":
                case "KA":
                case "KATAKANA":
                    return Katakana;

                default:
                    throw new FormatException(
                        $"Format: '{format}' not supported, only " +
                        "'R', 'H', 'K', 'RO', 'HI', 'KA', 'Roomaji', 'Hiragana' and 'Katakana' are supported");
            }
        }

        #endregion IFormattable Implementation
    }
}
