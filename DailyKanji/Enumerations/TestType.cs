namespace DailyKanji.Enumerations
{
    public enum TestType
    {
        /// <summary>
        /// Question in Hiragana or Katakana, Answer in Roomaji
        /// </summary>
        HiraganaOrKatakanaToRoomaji = 0,

        /// <summary>
        /// Question in Hiragana, Answer in Roomaji
        /// </summary>
        HiraganaToRoomaji = 1,

        /// <summary>
        /// Question in Katakana, Answer in Roomaji
        /// </summary>
        KatakanaToRoomaji = 2,

        /// <summary>
        /// Question in Roomaji, Answer in Hiragana
        /// </summary>
        RoomajiToHiragana = 3,

        /// <summary>
        /// Question in Roomaji, Answer in Katakana
        /// </summary>
        RoomajiToKatakana = 4
    }
}
