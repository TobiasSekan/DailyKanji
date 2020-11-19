namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// Type of statistic reset
    /// </summary>
    public enum ResetType
    {
        /// <summary>
        /// Reset the complete statistic
        /// </summary>
        All = 0,

        /// <summary>
        /// Reset only entries for correct answers
        /// </summary>
        OnlyCorrectAll = 1,

        /// <summary>
        /// Reset only entries for correct Hiragana answers
        /// </summary>
        OnlyCorrectHiragana = 2,

        /// <summary>
        /// Reset only entries for correct Katakana answers
        /// </summary>
        OnlyCorrectKatakana = 3,

        /// <summary>
        /// Reset only entries for wrong answers
        /// </summary>
        OnlyWrongAll = 4,

        /// <summary>
        /// Reset only entries for wrong Hiragana answers
        /// </summary>
        OnlyWrongHiragana = 5,

        /// <summary>
        /// Reset only entries for wrong Katakana answers
        /// </summary>
        OnlyWrongKatakana = 6
    }
}
