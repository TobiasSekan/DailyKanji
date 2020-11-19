namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// The type of the hint
    /// </summary>
    public enum HintType
    {
        /// <summary>
        /// The type of the hint is based on the ask sign
        /// </summary>
        BasedOnAskSign = 0,

        /// <summary>
        /// The hint is always in Roomaji
        /// </summary>
        AlwaysInRoomaji = 1,

        /// <summary>
        /// The hint is always in Hiragana
        /// </summary>
        AlwaysInHiragana = 2,

        /// <summary>
        /// The hint is always in Katakana
        /// </summary>
        AlwaysInKatakana = 3,
    }
}
