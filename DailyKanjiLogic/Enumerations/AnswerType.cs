namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// The type of a answer
    /// </summary>
    public enum AnswerType : byte
    {
        /// <summary>
        /// This answer type is unknown (e.g. used when answer is invalid or no answer is selected)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicate that the answer is in Roomaji
        /// </summary>
        Roomaji = 1,

        /// <summary>
        /// Indicate that the answer is in Hiragana
        /// </summary>
        Hiragana = 2,

        /// <summary>
        /// Indicate that the answer is in Katakana
        /// </summary>
        Katakana = 3,
    }
}
