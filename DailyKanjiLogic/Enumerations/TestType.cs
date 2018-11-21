namespace DailyKanjiLogic.Enumerations
{
    public enum TestType
    {
        /// <summary>
        /// Question in Hiragana or Katakana and answer in Roomaji
        /// </summary>
        HiraganaOrKatakanaToRoomaji = 0,

        /// <summary>
        /// Question in Hiragana and answer in Roomaji
        /// </summary>
        HiraganaToRoomaji = 1,

        /// <summary>
        /// Question in Katakana and answer in Roomaji
        /// </summary>
        KatakanaToRoomaji = 2,

        /// <summary>
        /// Question in Roomaji and answer in Katakana or Hiragana
        /// </summary>
        RoomajiToHiraganaOrKatakana = 3,

        /// <summary>
        /// Question in Roomaji and answer in Hiragana
        /// </summary>
        RoomajiToHiragana = 4,

        /// <summary>
        /// Question in Roomaji and answer in Katakana
        /// </summary>
        RoomajiToKatakana = 5,

        /// <summary>
        /// Question in Hiragana and answer in Katakana or Question in Katakana and answer in Hiragana
        /// </summary>
        HiraganaToKatakanaOrKatakanaOrHiragana = 6,

        /// <summary>
        /// Question in Hiragana and answer in Katakana
        /// </summary>
        HiraganaToKatakana = 7,

        /// <summary>
        /// Question in Katakana and answer in Hiragana
        /// </summary>
        KatakanaToHiragana = 8
    }
}
