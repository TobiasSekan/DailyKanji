namespace DailyKanji.Mvvm.Model
{
    public class TestBaseModel
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
        /// The count of wrong answers when the hiragana sign was ask
        /// </summary>
        public uint WrongHiragana { get; internal set; }

        /// <summary>
        /// The count of wrong answers when the katakana sign was ask
        /// </summary>
        public uint WrongKatakana { get; internal set; }

        public TestBaseModel(string roomaji, string hiragana, string katakana)
        {
            Roomaji  = roomaji;
            Hiragana = hiragana;
            Katakana = katakana;
        }

        protected TestBaseModel(TestBaseModel testBaseModel)
        {
            Roomaji       = testBaseModel.Roomaji;
            Hiragana      = testBaseModel.Hiragana;
            Katakana      = testBaseModel.Katakana;
            WrongHiragana = testBaseModel.WrongHiragana;
            WrongKatakana = testBaseModel.WrongKatakana;
        }
    }
}
