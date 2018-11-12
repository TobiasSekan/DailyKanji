using DailyKanji.Helper;
using Newtonsoft.Json;
using System;

namespace DailyKanji.Mvvm.Model
{
    public class TestBaseModel : PropertyChangedHelper
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
        public int WrongHiraganaCount
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
        public int WrongKatakanaCount
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
        public int CorrectHiraganaCount
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
        public int CorrectKatakanaCount
        {
            get => _correctKatakanaCount;
            set
            {
                _correctKatakanaCount = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan CompleteAnswerTimeForHiragana
        {
            get => _completeAnswerTimeForHiragana;
            set
            {
                _completeAnswerTimeForHiragana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForHiragana));
            }
        }

        public TimeSpan CompleteAnswerTimeForKatakana
        {
            get => _completeAnswerTimeForKatakana;
            set
            {
                _completeAnswerTimeForKatakana = value;
                OnPropertyChanged(nameof(AverageAnswerTimeForKatakana));
            }
        }

        [JsonIgnore]
        public TimeSpan AverageAnswerTimeForHiragana
            => CorrectHiraganaCount + WrongHiraganaCount > 0
                ? new TimeSpan(CompleteAnswerTimeForHiragana.Ticks / (CorrectHiraganaCount + WrongHiraganaCount))
                : new TimeSpan();

        [JsonIgnore]
        public TimeSpan AverageAnswerTimeForKatakana
            => CorrectKatakanaCount + WrongKatakanaCount > 0
                ? new TimeSpan(CompleteAnswerTimeForKatakana.Ticks / (CorrectKatakanaCount + WrongKatakanaCount))
                : new TimeSpan();

        #endregion Public Properties

        #region Private Backing-fields

        /// <summary>
        /// Backing-field for <see cref="WrongHiraganaCount"/>
        /// </summary>
        private int _wrongHiraganaCount;

        /// <summary>
        /// Backing-field for <see cref="WrongKatakanaCount"/>
        /// </summary>
        private int _wrongKatakanaCount;

        /// <summary>
        /// Backing-field for <see cref="CorrectHiraganaCount"/>
        /// </summary>
        private int _correctHiraganaCount;

        /// <summary>
        /// Backing-field for <see cref="CorrectKatakanaCount"/>
        /// </summary>
        private int _correctKatakanaCount;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForHiragana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForHiragana;

        /// <summary>
        /// Backing-field for <see cref="CompleteAnswerTimeForKatakana"/>
        /// </summary>
        private TimeSpan _completeAnswerTimeForKatakana;

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
            Roomaji  = roomaji;
            Hiragana = hiragana;
            Katakana = katakana;
        }

        /// <summary>
        /// Create a new test, based on the given <see cref="TestBaseModel"/>
        /// </summary>
        /// <param name="testBaseModel">The <see cref="TestBaseModel"/> for this test</param>
        protected TestBaseModel(TestBaseModel testBaseModel)
        {
            Roomaji                       = testBaseModel.Roomaji;
            Hiragana                      = testBaseModel.Hiragana;
            Katakana                      = testBaseModel.Katakana;
            WrongHiraganaCount            = testBaseModel.WrongHiraganaCount;
            WrongKatakanaCount            = testBaseModel.WrongKatakanaCount;
            CorrectHiraganaCount          = testBaseModel.CorrectHiraganaCount;
            CorrectKatakanaCount          = testBaseModel.CorrectKatakanaCount;
            CompleteAnswerTimeForHiragana = testBaseModel.CompleteAnswerTimeForHiragana;
            CompleteAnswerTimeForKatakana = testBaseModel.CompleteAnswerTimeForKatakana;
        }

        #endregion Constructors
    }
}
