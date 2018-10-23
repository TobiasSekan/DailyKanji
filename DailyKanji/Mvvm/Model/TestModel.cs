namespace DailyKanji.Mvvm.Model
{
    public sealed class TestModel
    {
        /// <summary>
        /// The sign in Roomaji
        /// </summary>
        public string Roomaji { get; }

        /// <summary>
        /// The sign in Hiragana
        /// </summary>
        public string Hiragana { get; }

        /// <summary>
        /// The sign in Katakana
        /// </summary>
        public string Katakana { get; }

        /// <summary>
        /// The count of wrong answers when the hiragana sign was ask
        /// </summary>
        public uint WrongHiragana { get; internal set; }

        /// <summary>
        /// The count of wrong answers when the katakana sign was ask
        /// </summary>
        public uint WrongKatakana { get; internal set; }

        public TestModel(string roomaji, string hiragana, string katakana)
        {
            Roomaji  = roomaji;
            Hiragana = hiragana;
            Katakana = katakana;
        }
    }
}
