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
        /// Only monograph kana (45 kana)
        /// </summary>
        Gojuuon = 1,

        /// <summary>
        /// Only monograph kana with two little strokes (20 kana)
        /// </summary>
        GojuuonWithDakuten = 1 << 1,

        /// <summary>
        /// Only monograph kana with a little circle (5 kana)
        /// </summary>
        GojuuonWithHandakuten = 1 << 2,

        /// <summary>
        /// Only digraph kana (21 kana)
        /// </summary>
        Yooon = 1 << 3,

        /// <summary>
        /// Only digraph kana with two little strokes (9 kana)
        /// </summary>
        YooonWithDakuten = 1 << 4,

        /// <summary>
        /// Only digraph kana with a little circle (3 kana)
        /// </summary>
        YooonWithHandakuten = 1 << 5,

        /// <summary>
        /// All possible kana types
        /// </summary>
        All = Gojuuon | GojuuonWithDakuten | GojuuonWithHandakuten | Yooon | YooonWithDakuten | YooonWithHandakuten
    }
}
