using System.Drawing;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier work with colors
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// The color string for the progress bar
        /// </summary>
        public static readonly Color ProgressBarColor = Color.LightBlue;

        /// <summary>
        /// The color string for the error highlight
        /// </summary>
        public static readonly Color ErrorColor = Color.LightCoral;

        /// <summary>
        /// The color string for marked answers
        /// </summary>
        public static readonly Color MarkedColor =  Color.LightGoldenrodYellow;

        /// <summary>
        /// The color string for the correct answer
        /// </summary>
        public static readonly Color CorrectColor = Color.LightGreen;

        /// <summary>
        /// The color string for invisible text and elements
        /// </summary>
        public static readonly Color TransparentColor = Color.Transparent;

        /// <summary>
        /// The color string for the answer hints
        /// </summary>
        public static readonly Color HintTextColor = Color.Black;
    }
}
