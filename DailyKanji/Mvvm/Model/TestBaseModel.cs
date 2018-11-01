using DailyKanji.Helper;

namespace DailyKanji.Mvvm.Model
{
    public class TestBaseModel : PropertyChangedHelper
    {
        /// <summary>
        /// The sign in Roomaji
        /// </summary>
        public string Roomaji { get; internal set; }

        /// <summary>
        /// The sign in Hiragana
        /// </summary>
        public string Hiragana { get; internal set; }

        /// <summary>
        /// The sign in Katakana
        /// </summary>
        public string Katakana { get; internal set; }

        /// <summary>
        /// The count of wrong answers when the Hiragana sign was ask
        /// </summary>
        public int WrongHiraganaCount
        {
            get => _wrongHiraganaCount;
            internal set
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
            internal set
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
            internal set
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
            internal set
            {
                _correctKatakanaCount = value;
                OnPropertyChanged();
            }
        }

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
        /// Create a new test, based on the given values
        /// </summary>
        /// <param name="roomaji">The sign in Roomaji</param>
        /// <param name="hiragana">The sign in Hiragana</param>
        /// <param name="katakana">The sign in Katakana</param>
        public TestBaseModel(string roomaji, string hiragana, string katakana)
        {
            Roomaji = roomaji;
            Hiragana = hiragana;
            Katakana = katakana;
        }

        /// <summary>
        /// Create a new test, based on the given <see cref="TestBaseModel"/>
        /// </summary>
        /// <param name="testBaseModel">The <see cref="TestBaseModel"/> for this test</param>
        protected TestBaseModel(TestBaseModel testBaseModel)
        {
            Roomaji = testBaseModel.Roomaji;
            Hiragana = testBaseModel.Hiragana;
            Katakana = testBaseModel.Katakana;
            WrongHiraganaCount = testBaseModel.WrongHiraganaCount;
            WrongKatakanaCount = testBaseModel.WrongKatakanaCount;
            CorrectHiraganaCount = testBaseModel.CorrectHiraganaCount;
            CorrectKatakanaCount = testBaseModel.CorrectKatakanaCount;
        }
    }
}
