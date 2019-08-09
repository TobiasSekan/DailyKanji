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
        None = 0x_00,

        /// <summary>
        /// Only monograph kana
        /// </summary>
        Gojuuon = 0x_01,

        /// <summary>
        /// Only monograph kana with two little strokes
        /// </summary>
        GojuuonWithDakuten = 0x_02,

        /// <summary>
        /// Only monograph kana with a little circle
        /// </summary>
        GojuuonWithHandakuten = 0x_04,

        /// <summary>
        /// Only digraph kana
        /// </summary>
        Yooon = 0x_08,

        /// <summary>
        /// Only digraph kana with two little strokes
        /// </summary>
        YooonWithDakuten = 0x_10,

        /// <summary>
        /// Only digraph kana with a little circle
        /// </summary>
        YooonWithHandakuten = 0x_20
    }
}
