using System;

namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// The type of the kana (enum used <see cref="FlagsAttribute"/>)
    /// </summary>
    [Flags]
    public enum KanaType : byte
    {
        /// <summary>
        /// Indicate that no kana type is selected
        /// </summary>
        None = 0,

        /// <summary>
        /// Only monograph kana
        /// </summary>
        Gojuuon = 1,

        /// <summary>
        /// Only monograph kana with two little strokes
        /// </summary>
        GojuuonWithDakuten = 1 << 1,

        /// <summary>
        /// Only monograph kana with a little circle
        /// </summary>
        GojuuonWithHandakuten = 1 << 2,

        /// <summary>
        /// Only digraph kana
        /// </summary>
        Yooon = 1 << 3,

        /// <summary>
        /// Only digraph kana with two little strokes
        /// </summary>
        YooonWithDakuten = 1 << 4,

        /// <summary>
        /// Only digraph kana with a little circle
        /// </summary>
        YooonWithHandakuten = 1 << 5
    }
}
