namespace DailyKanjiLogic.Enumerations
{
    public enum KanaType : byte
    {
        /// <summary>
        /// Monographs
        /// </summary>
        Gojuuon = 0x_00,

        /// <summary>
        /// Monographs with two little strokes
        /// </summary>
        GojuuonWithDakuten = 0x_01,

        /// <summary>
        /// Monographs with a little circle
        /// </summary>
        GojuuonWithHandakuten = 0x_02,

        /// <summary>
        /// Digraphs
        /// </summary>
        Yooon = 0x_03,

        /// <summary>
        /// Digraphs with two little strokes
        /// </summary>
        YooonWithDakuten = 0x_04,

        /// <summary>
        /// Digraphs with a little circle
        /// </summary>
        YooonWithHandakuten = 0x_05,
    }
}
